using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetUInt48 = NetworkingPrimitivesCore.NetInt<NetworkingPrimitivesCore.UInt48>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<MacAddress>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<MacAddress>))]
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
    public static explicit operator UInt48(MacAddress value) => (UInt48)value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MacAddress(UInt48 value) => new((NetUInt48)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MacAddress other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is MacAddress other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(MacAddress left, MacAddress right) => left._value == right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(MacAddress left, MacAddress right) => left._value != right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(MacAddress other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(MacAddress left, MacAddress right) => left._value < right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(MacAddress left, MacAddress right) => left._value > right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(MacAddress left, MacAddress right) => left._value <= right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(MacAddress left, MacAddress right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator ~(MacAddress value) => new(~value._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator &(MacAddress left, MacAddress right) => new(left._value & right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator |(MacAddress left, MacAddress right) => new(left._value | right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress operator ^(MacAddress left, MacAddress right) => new(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format) => this.ToString(MaxStringLength, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination, ReadOnlySpan<char> format = default) => FormattingHelper.Format(this, destination, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return TryFormat(destination, out charsWritten, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return MacAddressFormatter<TChar>.TryFormat(Bytes, destination, out charsWritten, format);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out MacAddress result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<MacAddress>()];
        if (MacAddressFormatter<TChar>.TryParse(source, addressBytes))
        {
            result = new(addressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out MacAddress result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<MacAddress, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MacAddress Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString(format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten, format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten, format);

    #endregion
}