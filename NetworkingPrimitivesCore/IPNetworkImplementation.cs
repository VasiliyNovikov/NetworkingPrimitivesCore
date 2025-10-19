using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
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
        switch (TryInitialize(address, prefix, strict, out Address, out Mask, out Prefix))
        {
            case InitializationResult.Ok:
                return;
            case InitializationResult.PrefixOutOfRange:
                throw new ArgumentOutOfRangeException(nameof(prefix));
            case InitializationResult.InvalidHostBits:
                throw new ArgumentException($"{address}/{prefix} has host bits set", nameof(address));
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
    public bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        if (!Address.TryFormat(destination, out charsWritten))
            return false;

        if (charsWritten == destination.Length)
            return false;

        destination[charsWritten++] = TChar.CreateTruncating('/');
        if (!Prefix.TryFormat(destination[charsWritten..], out var prefixCharsWritten, provider: CultureInfo.InvariantCulture))
            return false;

        charsWritten += prefixCharsWritten;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar>(ReadOnlySpan<TChar> source, bool strict, out IPNetworkImplementation<TAddress, TUInt> network)
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        var slashIndex = source.IndexOf(TChar.CreateTruncating('/'));
        if (slashIndex == -1)
        {
            if (TAddress.TryParse(source, out var address) &&
                TryInitialize(address, null, strict, out address, out var mask, out var prefix) == InitializationResult.Ok)
            {
                network = new(address, mask, prefix);
                return true;
            }
        }
        else if (TAddress.TryParse(source[..slashIndex], out var address) &&
                 FormattingHelper.TryParse<byte, TChar>(source[(slashIndex + 1)..], CultureInfo.InvariantCulture, out var prefix) &&
                 TryInitialize(address, prefix, strict, out address, out var mask, out _) == InitializationResult.Ok)
        {
            network = new(address, mask, prefix);
            return true;
        }

        network = default;
        return false;
    }

    private enum InitializationResult
    {
        Ok,
        PrefixOutOfRange,
        InvalidHostBits
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static InitializationResult TryInitialize(TAddress inputAddress, byte? inputPrefix, bool strict, out TAddress address, out TAddress mask, out byte prefix)
    {
        prefix = inputPrefix ?? BitSize;

        if (prefix > BitSize)
        {
            address = default;
            mask = default;
            prefix = 0;
            return InitializationResult.PrefixOutOfRange;
        }

        if (prefix == BitSize)
        {
            address = inputAddress;
            mask = TAddress.Broadcast;
            return InitializationResult.Ok;
        }

        if (prefix == 0)
        {
            address = default;
            mask = default;
        }
        else
        {
            mask = MaskCache[prefix];
            address = inputAddress & mask;
        }

        return inputAddress == address || !strict
            ? InitializationResult.Ok
            : InitializationResult.InvalidHostBits;
    }
}
