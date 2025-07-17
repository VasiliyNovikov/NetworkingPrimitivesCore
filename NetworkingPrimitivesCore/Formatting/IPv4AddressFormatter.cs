using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class IPv4AddressFormatter<TChar>
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    internal static TChar Separator => TChar.CreateTruncating('.');
    private static readonly TChar[][] DecSegments = new TChar[65536][];

    static IPv4AddressFormatter()
    {
        // Precompute Decimal Segments
        Span<TChar> buffer = stackalloc TChar[7];
        for (var s = 0; s < 65536; ++s)
        {
            var segment = (ushort)s;
            var segmentBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref segment, 1));
            var writer = new SpanWriter<TChar>(buffer);
            for (var i = 0; i < 2; ++i)
            {
                if (i > 0)
                    writer.TryWrite(Separator);

                var (firstAndSecondDigits, thirdDigit) = Math.DivRem(segmentBytes[i], (byte)10);
                var (firstDigit, secondDigit) = Math.DivRem(firstAndSecondDigits, (byte)10);

                if (firstDigit != 0)
                    writer.TryWriteDecimalDigit(firstDigit);

                if (secondDigit != 0 || firstDigit != 0)
                    writer.TryWriteDecimalDigit(secondDigit);

                writer.TryWriteDecimalDigit(thirdDigit);
            }
            DecSegments[s] = buffer[..writer.Position].ToArray();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<TChar> source, Span<byte> ipAddressBytes)
    {
        var reader = new SpanReader<TChar>(source);
        for (var i = 0; i < ipAddressBytes.Length; ++i)
        {
            if (i > 0 && !reader.TryReadOne(Separator))
                return false;

            if (!TryReadComponent(ref reader, out ipAddressBytes[i]))
                return false;
        }
        return reader.IsEndOfSource;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadComponent(ref SpanReader<TChar> reader, out byte component)
    {
        if (!reader.TryReadDecimalDigit(out var firstDigit))
        {
            Unsafe.SkipInit(out component);
            return false;
        }

        int componentInt = firstDigit;

        if (reader.TryReadDecimalDigit(out var secondDigit))
            componentInt = (componentInt << 3) + (componentInt << 1) + secondDigit;

        if (reader.TryReadDecimalDigit(out var thirdDigit))
            componentInt = (componentInt << 3) + (componentInt << 1) + thirdDigit;

        if (componentInt > byte.MaxValue)
        {
            Unsafe.SkipInit(out component);
            return false;
        }

        component = (byte)componentInt;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(ReadOnlySpan<byte> ipAddressBytes, Span<TChar> destination, out int charsWritten)
    {
        var writer = new SpanWriter<TChar>(destination);
        var result = TryWrite(ref writer, ipAddressBytes);
        charsWritten = writer.Position;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryWrite(ref SpanWriter<TChar> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        var addressSegments = MemoryMarshal.Cast<byte, ushort>(ipAddressBytes);
        return writer.TryWrite(DecSegments[addressSegments[0]]) &&
               writer.TryWrite(Separator) &&
               writer.TryWrite(DecSegments[addressSegments[1]]);
    }
}