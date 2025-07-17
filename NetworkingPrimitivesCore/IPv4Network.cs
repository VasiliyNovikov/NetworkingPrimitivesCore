using System;
using System.Runtime.CompilerServices;
using System.Numerics;
using NetworkingPrimitivesCore.Formatting;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Json;
using NetworkingPrimitivesCore.Converters;

using NetAddress = NetworkingPrimitivesCore.IPv4Address;
using IPNetworkImplementation = NetworkingPrimitivesCore.IPNetworkImplementation<NetworkingPrimitivesCore.IPv4Address, uint>;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<IPv4Network>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<IPv4Network>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct IPv4Network : IIPNetwork<IPv4Network, NetAddress, uint>
{
    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IPNetworkImplementation.MaxStringLength;
    }

    public static int Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => NetAddress.Version;
    }

    private readonly IPNetworkImplementation _implementation;

    public NetAddress Address
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _implementation.Address;
    }

    public NetAddress Mask
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _implementation.Mask;
    }

    public byte Prefix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _implementation.Prefix;
    }

    public NetAddress Gateway
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _implementation.Gateway;
    }

    public NetAddress Broadcast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _implementation.Broadcast;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IPv4Network(IPNetworkImplementation implementation) => _implementation = implementation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Network(NetAddress address, byte? prefix = null, bool strict = true) : this(new(address, prefix, strict)) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(NetAddress address) => _implementation.Contains(address);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NetAddress AddressAt<TIndex>(TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex> => _implementation.AddressAt(index);

    public NetAddress this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public NetAddress this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Network Subnet<TIndex>(byte prefix, TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex> => new(_implementation.Subnet(prefix, index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Network Supernet(byte prefix) => new(_implementation.Supernet(prefix));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _implementation.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPv4Network other) => _implementation.Equals(other._implementation);

    public override bool Equals(object? obj) => obj is IPv4Network other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPv4Network left, IPv4Network right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPv4Network left, IPv4Network right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPv4Network other) => _implementation.CompareTo(other._implementation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPv4Network a, IPv4Network b) => a.CompareTo(b) < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPv4Network a, IPv4Network b) => a.CompareTo(b) > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPv4Network a, IPv4Network b) => a.CompareTo(b) <= 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPv4Network a, IPv4Network b) => a.CompareTo(b) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FormattingHelper.ToString(this, MaxStringLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination) => FormattingHelper.Format(this, destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return _implementation.TryFormat(destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out IPv4Network result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        if (IPNetworkImplementation.TryParse(source, out var implementation))
        {
            result = new(implementation);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out IPv4Network result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Network Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<IPv4Network, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPv4Network Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten);

    #endregion
}