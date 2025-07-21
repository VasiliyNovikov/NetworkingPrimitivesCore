using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetUInt32 = NetworkingPrimitivesCore.NetInt<uint>;
using OperatorHelper = NetworkingPrimitivesCore.NetIntConvertibleOperatorHelper<NetworkingPrimitivesCore.IPv4Address, uint>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetAddressConverter<IPv4Address>))]
[TypeConverter(typeof(NetAddressConverter<IPv4Address>))]
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
    public static explicit operator uint(IPv4Address value) => OperatorHelper.ToInt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv4Address(uint value) => OperatorHelper.FromInt(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => OperatorHelper.GetHashCode(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPv4Address other) => OperatorHelper.Equal(this, other);
    public override bool Equals(object? obj) => obj is IPv4Address other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPv4Address left, IPv4Address right) => OperatorHelper.Equal(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPv4Address left, IPv4Address right) => OperatorHelper.NotEqual(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPv4Address other) => OperatorHelper.Compare(this, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPv4Address left, IPv4Address right) => OperatorHelper.LessThan(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPv4Address left, IPv4Address right) => OperatorHelper.GreaterThan(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPv4Address left, IPv4Address right) => OperatorHelper.LessThanOrEqual(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPv4Address left, IPv4Address right) => OperatorHelper.GreaterThanOrEqual(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator ~(IPv4Address value) => OperatorHelper.Not(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator &(IPv4Address left, IPv4Address right) => OperatorHelper.And(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator |(IPv4Address left, IPv4Address right) => OperatorHelper.Or(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address operator ^(IPv4Address left, IPv4Address right) => OperatorHelper.Xor(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FormattingHelper.ToString(this, MaxStringLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination) => FormattingHelper.Format(this, destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten) => IPv4AddressFormatter.TryFormat(Bytes, destination, out charsWritten);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> source, out IPv4Address result)
    {
        Span<byte> ipAddressBytes = stackalloc byte[Unsafe.SizeOf<IPv4Address>()];
        if (IPv4AddressFormatter.TryParse(source, ipAddressBytes))
        {
            result = new IPv4Address(ipAddressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address Parse(ReadOnlySpan<char> source) => FormattingHelper.Parse<IPv4Address>(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Address Parse(string source) => FormattingHelper.Parse<IPv4Address>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);

    #endregion
}