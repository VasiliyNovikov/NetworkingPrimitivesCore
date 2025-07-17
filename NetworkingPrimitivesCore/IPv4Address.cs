using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetUInt32 = NetworkingPrimitivesCore.NetInt<uint>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<IPv4Address>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<IPv4Address>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct IPv4Address : IIPAddress<IPv4Address, uint>
{
    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 15; // 4 bytes * 3 digits + 3 separators (e.g., "255.255.255.255")
    }

    public static int Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }

    public static IPv4Address Loopback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (IPv4Address)(NetUInt32)0x7F_00_00_01u;
    }

    public static IPv4Address Any
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    private readonly NetUInt32 _value;

    public ReadOnlySpan<byte> Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in this), 1));
    }

    public bool IsLinkLocal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Bytes[0] == 169 && Bytes[1] == 254;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IPv4Address(NetUInt32 value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Address(ReadOnlySpan<byte> addressBytes) => _value = MemoryMarshal.Read<NetUInt32>(addressBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetUInt32(IPv4Address value) => value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv4Address(NetUInt32 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(IPv4Address value) => (uint)value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv4Address(uint value) => new((NetUInt32)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPv4Address other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is IPv4Address other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPv4Address left, IPv4Address right) => left._value == right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPv4Address left, IPv4Address right) => left._value != right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPv4Address other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPv4Address left, IPv4Address right) => left._value < right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPv4Address left, IPv4Address right) => left._value > right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPv4Address left, IPv4Address right) => left._value <= right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPv4Address left, IPv4Address right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator ~(IPv4Address value) => new(~value._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator &(IPv4Address left, IPv4Address right) => new(left._value & right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator |(IPv4Address left, IPv4Address right) => new(left._value | right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator ^(IPv4Address left, IPv4Address right) => new(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FormattingHelper.ToString(this, MaxStringLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination) => FormattingHelper.Format(this, destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return IPv4AddressFormatter<TChar>.TryFormat(Bytes, destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out IPv4Address result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<IPv4Address>()];
        if (IPv4AddressFormatter<TChar>.TryParse(source, addressBytes))
        {
            result = new(addressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out IPv4Address result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<IPv4Address, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten);

    #endregion
}