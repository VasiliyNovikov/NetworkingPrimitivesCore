using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<IPNetwork>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<IPNetwork>))]
[StructLayout(LayoutKind.Explicit, Pack = 4)]
public readonly struct IPNetwork : IIPNetworkBase<IPNetwork, IPAnyAddress>
{
    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IPv6Network.MaxStringLength;
    }

    [FieldOffset(0)]
    private readonly IPv4Network _ipv4Network;
    [FieldOffset(0)]
    private readonly IPv6Network _ipv6Network;
    [FieldOffset(36)]
    private readonly bool _isV6;

    public IPAnyAddress Address
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Network.Address : _ipv4Network.Address;
    }

    public IPAnyAddress Mask
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Network.Mask : _ipv4Network.Mask;
    }

    public byte Prefix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Network.Prefix : _ipv4Network.Prefix;
    }

    public IPAnyAddress Gateway
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Network.Gateway : _ipv4Network.Gateway;
    }

    public IPAnyAddress Broadcast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Network.Broadcast : _ipv4Network.Broadcast;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetwork(IPAnyAddress address, byte? prefix = null, bool strict = true)
    {
        _isV6 = address.Version == 6;
        if (_isV6)
            _ipv6Network = new IPv6Network((IPv6Address)address, prefix, strict);
        else
            _ipv4Network = new IPv4Network((IPv4Address)address, prefix, strict);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetwork(IPv4Network network)
    {
        _isV6 = false;
        _ipv4Network = network;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetwork(IPv6Network network)
    {
        _isV6 = true;
        _ipv6Network = network;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv4Network(IPNetwork network)
    {
        return network._isV6
            ? throw new InvalidCastException($"Cannot cast IPv6 network {network} to IPv4 network")
            : network._ipv4Network;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv6Network(IPNetwork network)
    {
        return network._isV6
            ? network._ipv6Network
            : throw new InvalidCastException($"Cannot cast IPv4 network {network} to IPv6 network");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IPNetwork(IPv4Network network) => new(network);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IPNetwork(IPv6Network network) => new(network);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(IPAnyAddress address)
    {
        return address.Version == 6 == _isV6 && (_isV6 ? _ipv6Network.Contains((IPv6Address)address) : _ipv4Network.Contains((IPv4Address)address));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPAnyAddress AddressAt<TIndex>(TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>
    {
        return _isV6 ? _ipv6Network.AddressAt(index) : _ipv4Network.AddressAt(index);
    }

    public IPAnyAddress this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public IPAnyAddress this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public IPAnyAddress this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public IPAnyAddress this[ulong index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public IPAnyAddress this[Int128 index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    public IPAnyAddress this[UInt128 index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetwork Subnet<TIndex>(byte prefix, TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>
    {
        return _isV6 ? _ipv6Network.Subnet(prefix, index) : _ipv4Network.Subnet(prefix, index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetwork Supernet(byte prefix) => _isV6 ? _ipv6Network.Supernet(prefix) : _ipv4Network.Supernet(prefix);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _isV6 ? _ipv6Network.GetHashCode() : _ipv4Network.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPNetwork other) => Address == other.Address && Mask == other.Mask;

    public override bool Equals(object? obj) => obj is IPNetwork other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPNetwork a, IPNetwork b) => a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPNetwork a, IPNetwork b) => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPNetwork other)
    {
        return _isV6 == other._isV6
            ? (_isV6
                   ? _ipv6Network.CompareTo(other._ipv6Network)
                   : _ipv4Network.CompareTo(other._ipv4Network))
            : _isV6.CompareTo(other._isV6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPNetwork a, IPNetwork b) => a.CompareTo(b) < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPNetwork a, IPNetwork b) => a.CompareTo(b) > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPNetwork a, IPNetwork b) => a.CompareTo(b) <= 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPNetwork a, IPNetwork b) => a.CompareTo(b) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FormattingHelper.ToString(this, MaxStringLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Format(Span<char> destination) => FormattingHelper.Format(this, destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return _isV6
            ? _ipv6Network.TryFormat(destination, out charsWritten)
            : _ipv4Network.TryFormat(destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out IPNetwork result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        if (source.Contains(TChar.CreateTruncating(':')))
        {
            if (IPv6Network.TryParse(source, out var resultV6))
            {
                result = resultV6;
                return true;
            }
        }
        else
        {
            if (IPv4Network.TryParse(source, out var resultV4))
            {
                result = resultV4;
                return true;
            }
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out IPNetwork result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPNetwork Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<IPNetwork, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPNetwork Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten);

    #endregion
}