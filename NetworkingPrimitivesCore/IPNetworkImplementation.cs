using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct IPNetworkImplementation<TAddress, TUInt>
    where TAddress : unmanaged, IIPAddress<TAddress, TUInt>, INetIntConvertible<TAddress, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>
{
    private static readonly TUInt[] IntMaskCache = [.. Enumerable.Range(0, BitSize + 1).Select(prefix => TUInt.AllBitsSet << (BitSize - prefix))];
    private static readonly TUInt[] IntHostMaskCache = [.. IntMaskCache.Select(mask => ~mask)];
    private static readonly TAddress[] MaskCache = [.. IntMaskCache.Select(mask => (TAddress)mask)];

    private static byte BitSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (byte)(Unsafe.SizeOf<TUInt>() * 8);
    }

    public static int MaxStringLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TAddress.MaxStringLength + 1 + (BitSize > 99 ? 3 : 2);
    }

    public readonly TAddress Address;
    public readonly TAddress Mask;
    public readonly byte Prefix;

    public TAddress Gateway
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(TUInt.One);
    }

    public TAddress Broadcast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AddressAt(IntHostMaskCache[Prefix]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IPNetworkImplementation(TAddress address, TAddress mask, byte prefix)
    {
        Address = address;
        Mask = mask;
        Prefix = prefix;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetworkImplementation(TAddress address, byte? prefix, bool strict = true)
    {
        Address = address;
        switch (TryInitialize(address, prefix, out Mask, out Prefix))
        {
            case InitializationResult.OK:
                return;
            case InitializationResult.PrefixOutOfRange:
                throw new ArgumentOutOfRangeException(nameof(prefix));
            case InitializationResult.InvalidHostBits:
                if (strict)
                    throw new ArgumentException($"{address}/{prefix} has host bits set", nameof(address));
                Address &= Mask;
                return;
            default:
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(TAddress address) => Prefix == 0 || (address & Mask) == Address;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TAddress AddressAt<TIndex>(TIndex index)
        where TIndex : unmanaged, IBinaryInteger<TIndex>
    {
        var intIndex = TUInt.CreateChecked(index);
        return intIndex > IntHostMaskCache[Prefix]
            ? throw new ArgumentOutOfRangeException(nameof(index))
            : intIndex == TUInt.Zero
                ? Address
                : (TAddress)((TUInt)Address + intIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetworkImplementation<TAddress, TUInt> Subnet<TIndex>(byte subnetPrefix, TIndex index)
        where TIndex : unmanaged, IBinaryInteger<TIndex>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(subnetPrefix, Prefix);
        var subnetAddress = AddressAt(TUInt.CreateChecked(index) << (BitSize - subnetPrefix));
        return new IPNetworkImplementation<TAddress, TUInt>(subnetAddress, subnetPrefix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPNetworkImplementation<TAddress, TUInt> Supernet(byte supernetPrefix)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(supernetPrefix, Prefix);
        var supernetMask = MaskCache[supernetPrefix];
        var supernetAddress = Address & supernetMask;
        return new IPNetworkImplementation<TAddress, TUInt>(supernetAddress, supernetMask, supernetPrefix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(Address, Prefix);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IPNetworkImplementation<TAddress, TUInt> other) => Address == other.Address && Prefix == other.Prefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IPNetworkImplementation<TAddress, TUInt> other)
    {
        var addressComparison = Address.CompareTo(other.Address);
        return addressComparison != 0
            ? addressComparison
            : -Prefix.CompareTo(other.Prefix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        if (!Address.TryFormat(destination, out charsWritten))
            return false;

        if (charsWritten == destination.Length)
            return false;

        destination[charsWritten++] = '/';
        if (!Prefix.TryFormat(destination[charsWritten..], out var prefixCharsWritten, provider: CultureInfo.InvariantCulture))
            return false;

        charsWritten += prefixCharsWritten;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> s, out IPNetworkImplementation<TAddress, TUInt> network)
    {
        var slashIndex = s.IndexOf('/');
        if (slashIndex == -1)
        {
            if (TAddress.TryParse(s, out var address) &&
                TryInitialize(address, null, out var mask, out var prefix) == InitializationResult.OK)
            {
                network = new(address, mask, prefix);
                return true;
            }
        }
        else if (TAddress.TryParse(s[..slashIndex], out var address) &&
                 byte.TryParse(s[(slashIndex + 1)..], out var prefix) &&
                 TryInitialize(address, prefix, out var mask, out _) == InitializationResult.OK)
        {
            network = new(address, mask, prefix);
            return true;
        }

        network = default;
        return false;
    }

    private enum InitializationResult
    {
        OK,
        PrefixOutOfRange,
        InvalidHostBits
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static InitializationResult TryInitialize(TAddress address, byte? optionalPrefix, out TAddress mask, out byte prefix)
    {
        prefix = optionalPrefix ?? BitSize;

        if (prefix > BitSize)
        {
            mask = default;
            prefix = default;
            return InitializationResult.PrefixOutOfRange;
        }

        if (prefix == BitSize)
        {
            mask = TAddress.Broadcast;
            return InitializationResult.OK;
        }

        if (prefix == 0)
        {
            mask = default;
            return address == default
                ? InitializationResult.OK
                : InitializationResult.InvalidHostBits;
        }

        mask = MaskCache[prefix];
        return (address & mask) == address
            ? InitializationResult.OK
            : InitializationResult.InvalidHostBits;
    }
}
