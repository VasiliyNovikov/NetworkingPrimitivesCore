using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

using NetInt48 = NetworkingPrimitivesCore.NetInt<NetworkingPrimitivesCore.UInt48>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonMacAddressConverter))]
[TypeConverter(typeof(MacAddressCoverter))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct MacAddress : INetAddress<MacAddress, UInt48>
{
    public const int StringLength = 17; // 6 bytes * 2 hex digits + 5 separators (e.g., "00:00:00:00:00:00")

    private readonly NetInt48 _value;

    public ReadOnlySpan<byte> Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in _value), 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MacAddress(NetInt48 value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MacAddress(ReadOnlySpan<byte> addressBytes) => _value = MemoryMarshal.Read<NetInt48>(addressBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MacAddress other) => _value == other._value;
    public override bool Equals(object? obj) => obj is MacAddress other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(MacAddress a, MacAddress b) => a._value == b._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(MacAddress a, MacAddress b) => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(MacAddress other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(MacAddress a, MacAddress b) => a._value < b._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(MacAddress a, MacAddress b) => a._value > b._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(MacAddress a, MacAddress b) => a._value <= b._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(MacAddress a, MacAddress b) => a._value >= b._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetInt48(MacAddress value) => value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MacAddress(NetInt48 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format) => FormattingHelper.ToString(this, StringLength, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination, ReadOnlySpan<char> format = default) => FormattingHelper.Format(this, destination, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten) => MacAddressFormatter.TryFormat(Bytes, destination, out charsWritten, default);

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

    #region ISpanFormattable, IFormattable, ISpanParsable and IParsable implementations

    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString(format);
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten, format);
    static bool ISpanParsable<MacAddress>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out MacAddress result) => TryParse(s, out result);
    static bool IParsable<MacAddress>.TryParse(string? s, IFormatProvider? provider, out MacAddress result) => TryParse(s.AsSpan(), out result);
    static MacAddress ISpanParsable<MacAddress>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);
    static MacAddress IParsable<MacAddress>.Parse(string s, IFormatProvider? provider) => Parse(s);

    #endregion
}