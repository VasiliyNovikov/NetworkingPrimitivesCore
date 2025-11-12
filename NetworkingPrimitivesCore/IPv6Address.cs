using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetUInt128 = NetworkingPrimitivesCore.NetInt<System.UInt128>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<IPv6Address>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<IPv6Address>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct IPv6Address : IIPAddress<IPv6Address, UInt128>
{
    private static readonly NetUInt128 MappedIPv4Mask = (NetUInt128)new UInt128(UInt64.MaxValue, 0xFF_FF_FF_FF_00_00_00_00UL);
    private static readonly NetUInt128 MappedIPv4Prefix = (NetUInt128)new UInt128(0UL, 0x00_00_FF_FF_00_00_00_00UL);

    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 39; // 16 bytes * 4 hex digits + 15 separators (e.g., "2001:0db8:85a3:0000:0000:8a2e:0370:7334")
    }

    public static byte Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 6;
    }

    public static IPv6Address Broadcast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (IPv6Address)UInt128.MaxValue;
    }

    public static IPv6Address Loopback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (IPv6Address)(NetUInt128)UInt128.One;
    }

    public static IPv6Address Any
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    private readonly NetUInt128 _value;

    public ReadOnlySpan<byte> Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in this), 1));
    }

    public bool IsLinkLocal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Bytes[0] == 0xFE && Bytes[1] == 0x80;
    }

    public bool IsMulticast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Bytes[0] == 0xFF;
    }

    public bool IsIPv4MappedToIPv6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_value & MappedIPv4Mask) == MappedIPv4Prefix;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IPv6Address(NetUInt128 value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv6Address(ReadOnlySpan<byte> addressBytes) => _value = MemoryMarshal.Read<NetUInt128>(addressBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetUInt128(IPv6Address address) => address._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv6Address(NetUInt128 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Address MapToIPv4() => new(Bytes.Slice(12, 4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt128(IPv6Address value) => (UInt128)value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv6Address(UInt128 value) => new((NetUInt128)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPv6Address other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is IPv6Address other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPv6Address left, IPv6Address right) => left._value == right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPv6Address left, IPv6Address right) => left._value != right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPv6Address other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPv6Address left, IPv6Address right) => left._value < right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPv6Address left, IPv6Address right) => left._value > right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPv6Address left, IPv6Address right) => left._value <= right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPv6Address left, IPv6Address right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address operator ~(IPv6Address value) => new(~value._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address operator &(IPv6Address left, IPv6Address right) => new(left._value & right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address operator |(IPv6Address left, IPv6Address right) => new(left._value | right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address operator ^(IPv6Address left, IPv6Address right) => new(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToString(MaxStringLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format<TChar>(Span<TChar> destination)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Format(this, destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return IPv6AddressFormatter<TChar>.TryFormat(Bytes, IsIPv4MappedToIPv6, destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out IPv6Address result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<IPv6Address>()];
        if (IPv6AddressFormatter<TChar>.TryParse(source, addressBytes))
        {
            result = new(addressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out IPv6Address result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<IPv6Address, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv6Address Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten);

    #endregion
}