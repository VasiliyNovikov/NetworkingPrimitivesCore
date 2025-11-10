using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Converters;
using NetworkingPrimitivesCore.Formatting;
using NetworkingPrimitivesCore.Json;

namespace NetworkingPrimitivesCore;

[JsonConverter(typeof(JsonNetPrimitiveConverter<IPAnyAddress>))]
[TypeConverter(typeof(NetPrimitiveTypeConverter<IPAnyAddress>))]
[StructLayout(LayoutKind.Explicit, Pack = 4)]
public readonly struct IPAnyAddress
    : IIPAddressBase<IPAnyAddress>
    , INetIntConvertible<IPAnyAddress, uint>
    , INetIntConvertible<IPAnyAddress, UInt128>
{
    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IPv6Address.MaxStringLength;
    }

    [FieldOffset(0)]
    private readonly IPv4Address _ipv4Address;
    [FieldOffset(0)]
    private readonly IPv6Address _ipv6Address;
    [FieldOffset(16)]
    private readonly bool _isV6;

    public ReadOnlySpan<byte> Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Address.Bytes : _ipv4Address.Bytes;
    }

    public bool IsV6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6;
    }

    public byte Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? IPv6Address.Version : IPv4Address.Version;
    }

    public AddressFamily AddressFamily
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
    }

    public bool IsIPv4MappedToIPv6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 && _ipv6Address.IsIPv4MappedToIPv6;
    }

    public bool IsLinkLocal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isV6 ? _ipv6Address.IsLinkLocal : _ipv4Address.IsLinkLocal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPAnyAddress(ReadOnlySpan<byte> addressBytes)
    {
        if (addressBytes.Length == Unsafe.SizeOf<IPv6Address>())
        {
            _isV6 = true;
            _ipv6Address = new IPv6Address(addressBytes);
        }
        else
        {
            _isV6 = false;
            _ipv4Address = new IPv4Address(addressBytes);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPAnyAddress(IPv4Address address)
    {
        _isV6 = false;
        _ipv4Address = address;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPAnyAddress(IPv6Address address)
    {
        _isV6 = true;
        _ipv6Address = address;
    }

    public IPAnyAddress(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        _isV6 = address.AddressFamily == AddressFamily.InterNetworkV6;
        Span<byte> buffer = stackalloc byte[_isV6 ? Unsafe.SizeOf<IPv6Address>() : Unsafe.SizeOf<IPv4Address>()];

        if (!address.TryWriteBytes(buffer, out _))
            throw new ArgumentException($"Cannot construct IPAnyAddress from System.Net.IPAddress {address}");

        if (_isV6)
            _ipv6Address = new IPv6Address(buffer);
        else
            _ipv4Address = new IPv4Address(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv4Address(IPAnyAddress address)
    {
        return address._isV6
            ? throw new InvalidCastException($"Cannot cast IPv6 address {address} to IPv4 address")
            : address._ipv4Address;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPv6Address(IPAnyAddress address)
    {
        return address._isV6
            ? address._ipv6Address
            : throw new InvalidCastException($"Cannot cast IPv4 address {address} to IPv6 address");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPAddress(IPAnyAddress address) => new(address.Bytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IPAnyAddress(IPv4Address address) => new(address);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IPAnyAddress(IPv6Address address) => new(address);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IPAnyAddress(IPAddress address) => new(address);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPAnyAddress(NetInt<uint> address) => (IPv4Address)address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator IPAnyAddress(NetInt<UInt128> address) => (IPv6Address)address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetInt<uint>(IPAnyAddress address) => (NetInt<uint>)(IPv4Address)address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetInt<UInt128>(IPAnyAddress address) => (NetInt<UInt128>)(IPv6Address)address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPv4Address MapToIPv4() => _isV6 ? _ipv6Address.MapToIPv4() : _ipv4Address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _isV6 ? _ipv6Address.GetHashCode() : _ipv4Address.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPAnyAddress other)
    {
        return _isV6 == other._isV6 && (_isV6 ? _ipv6Address.Equals(other._ipv6Address) : _ipv4Address.Equals(other._ipv4Address));
    }

    public override bool Equals(object? obj) => obj is IPAnyAddress other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IPAnyAddress a, IPAnyAddress b) => a.Equals(b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IPAnyAddress a, IPAnyAddress b) => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPAnyAddress other)
    {
        return _isV6 == other._isV6
            ? (_isV6
                   ? _ipv6Address.CompareTo(other._ipv6Address)
                   : _ipv4Address.CompareTo(other._ipv4Address))
            : _isV6.CompareTo(other._isV6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IPAnyAddress a, IPAnyAddress b) => a.CompareTo(b) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IPAnyAddress a, IPAnyAddress b) => a.CompareTo(b) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(IPAnyAddress a, IPAnyAddress b) => a.CompareTo(b) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(IPAnyAddress a, IPAnyAddress b) => a.CompareTo(b) >= 0;

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
        return _isV6
            ? _ipv6Address.TryFormat(destination, out charsWritten)
            : _ipv4Address.TryFormat(destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, out IPAnyAddress result)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        if (source.Contains(TChar.CreateTruncating(':')))
        {
            Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<IPv6Address>()];
            if (IPv6AddressFormatter<TChar>.TryParse(source, addressBytes))
            {
                result = new IPv6Address(addressBytes);
                return true;
            }
        }
        else
        {
            Span<byte> addressBytes = stackalloc byte[Unsafe.SizeOf<IPv4Address>()];
            if (IPv4AddressFormatter<TChar>.TryParse(source, addressBytes))
            {
                result = new IPv4Address(addressBytes);
                return true;
            }
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string source, out IPAnyAddress result) => TryParse<char>(source, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPAnyAddress Parse<TChar>(ReadOnlySpan<TChar> source)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return FormattingHelper.Parse<IPAnyAddress, TChar>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPAnyAddress Parse(string source) => Parse<char>(source);

    #region ISpanFormattable, IFormattable implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string IFormattable.ToString(string? format, IFormatProvider? provider) => ToString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten);

    #endregion
}