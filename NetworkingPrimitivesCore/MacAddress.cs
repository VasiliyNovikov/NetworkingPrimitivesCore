using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetUInt48 = NetworkingPrimitivesCore.NetInt<NetworkingPrimitivesCore.UInt48>;
using OperatorHelper = NetworkingPrimitivesCore.NetIntConvertibleOperatorHelper<NetworkingPrimitivesCore.MacAddress, NetworkingPrimitivesCore.UInt48>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetAddressConverter<MacAddress>))]
[TypeConverter(typeof(NetAddressConverter<MacAddress>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct MacAddress : INetAddress<MacAddress, UInt48>
{
    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 17; // 6 bytes * 2 hex digits + 5 separators (e.g., "00:00:00:00:00:00")
    }

    private readonly NetUInt48 _value;

    public ReadOnlySpan<byte> Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in this), 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MacAddress(NetUInt48 value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MacAddress(ReadOnlySpan<byte> addressBytes) => _value = MemoryMarshal.Read<NetUInt48>(addressBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetUInt48(MacAddress value) => value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MacAddress(NetUInt48 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(MacAddress value) => OperatorHelper.ToInt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MacAddress(UInt48 value) => OperatorHelper.FromInt(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => OperatorHelper.GetHashCode(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MacAddress other) => OperatorHelper.Equal(this, other);
    public override bool Equals(object? obj) => obj is MacAddress other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(MacAddress left, MacAddress right) => OperatorHelper.Equal(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(MacAddress left, MacAddress right) => OperatorHelper.NotEqual(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(MacAddress other) => OperatorHelper.Compare(this, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(MacAddress left, MacAddress right) => OperatorHelper.LessThan(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(MacAddress left, MacAddress right) => OperatorHelper.GreaterThan(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(MacAddress left, MacAddress right) => OperatorHelper.LessThanOrEqual(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(MacAddress left, MacAddress right) => OperatorHelper.GreaterThanOrEqual(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator ~(MacAddress value) => OperatorHelper.Not(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator &(MacAddress left, MacAddress right) => OperatorHelper.And(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator |(MacAddress left, MacAddress right) => OperatorHelper.Or(left, right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator ^(MacAddress left, MacAddress right) => OperatorHelper.Xor(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format) => FormattingHelper.ToString(this, MaxStringLength, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination, ReadOnlySpan<char> format = default) => FormattingHelper.Format(this, destination, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten) => TryFormat(destination, out charsWritten, default);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format) => MacAddressFormatter.TryFormat(Bytes, destination, out charsWritten, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> source, out MacAddress result)
    {
        Span<byte> macAddressBytes = stackalloc byte[Unsafe.SizeOf<MacAddress>()];
        if (MacAddressFormatter.TryParse(source, macAddressBytes))
        {
            result = new MacAddress(macAddressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress Parse(ReadOnlySpan<char> source) => FormattingHelper.Parse<MacAddress>(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress Parse(string source) => FormattingHelper.Parse<MacAddress>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString(format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten, format);

    #endregion
}