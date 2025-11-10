using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class IPNetworkFormatter<TChar, TAddress, TUInt>
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    where TAddress : unmanaged, IIPAddress<TAddress, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>
{
    private static readonly TChar[][] Prefixes = new TChar[129][];

    static IPNetworkFormatter()
    {
        // Precompute Prefixes
        Span<TChar> buffer = stackalloc TChar[4];
        buffer[0] = TChar.CreateTruncating('/');
        for (var prefix = 0; prefix < Prefixes.Length; ++prefix)
        {
            prefix.TryFormat(buffer[1..], out var charsWritten, provider: CultureInfo.InvariantCulture);
            Prefixes[prefix] = buffer[..(charsWritten + 1)].ToArray();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(TAddress address, byte prefix, Span<TChar> destination, out int charsWritten)
    {
        Span<TChar> prefixChars = Prefixes[prefix];
        if (address.TryFormat(destination, out charsWritten) && prefixChars.TryCopyTo(destination[charsWritten..]))
        {
            charsWritten += prefixChars.Length;
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<TChar> source, out TAddress address, out byte? prefix)
    {
        var slashIndex = source.IndexOf(TChar.CreateTruncating('/'));
        if (slashIndex == -1)
        {
            if (TAddress.TryParse(source, out address))
            {
                prefix = null;
                return true;
            }
        }
        else if (TAddress.TryParse(source[..slashIndex], out address) &&
                 FormattingHelper.TryParse<byte, TChar>(source[(slashIndex + 1)..], CultureInfo.InvariantCulture, out var p))
        {
            prefix = p;
            return true;
        }
        address = default;
        prefix = null;
        return false;
    }
}