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

[JsonConverter(typeof(JsonNetPrimitiveConverter<MACAddress>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<MACAddress>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct MACAddress : INetAddress<MACAddress, UInt48>
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
    private MACAddress(NetUInt48 value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MACAddress(ReadOnlySpan<byte> addressBytes) => _value = MemoryMarshal.Read<NetUInt48>(addressBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetUInt48(MACAddress value) => value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MACAddress(NetUInt48 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(MACAddress value) => (UInt48)value._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator MACAddress(UInt48 value) => new((NetUInt48)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MACAddress other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is MACAddress other && this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(MACAddress left, MACAddress right) => left._value == right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(MACAddress left, MACAddress right) => left._value != right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(MACAddress other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(MACAddress left, MACAddress right) => left._value < right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(MACAddress left, MACAddress right) => left._value > right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(MACAddress left, MACAddress right) => left._value <= right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(MACAddress left, MACAddress right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress operator ~(MACAddress value) => new(~value._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress operator &(MACAddress left, MACAddress right) => new(left._value & right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress operator |(MACAddress left, MACAddress right) => new(left._value | right._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress operator ^(MACAddress left, MACAddress right) => new(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format) => this.ToString(MaxStringLength, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format<TChar>(Span<TChar> destination)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Format(this, destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format<TChar>(Span<TChar> destination, ReadOnlySpan<char> format)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Format(this, destination, format);
    }

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
        return MACAddressFormatter<TChar>.TryFormat(Bytes, destination, out charsWritten, format);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out MACAddress result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<MACAddress>()];
        if (MACAddressFormatter<TChar>.TryParse(source, addressBytes))
        {
            result = new(addressBytes);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out MACAddress result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<MACAddress, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MACAddress Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString(format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten, format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten, format);

    #endregion
}