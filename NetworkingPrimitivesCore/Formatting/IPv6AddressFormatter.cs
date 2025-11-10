using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class IPv6AddressFormatter<TChar>
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    private static TChar Separator => TChar.CreateTruncating(':');
    private static readonly TChar[] IPv4MappedToIPv6PrefixStr = [.. "::ffff:".Select(TChar.CreateTruncating)];
    private static readonly byte[] IPv4MappedToIPv6Prefix = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF];
    private static readonly (byte Start, byte Length)[] ZeroSequences = new (byte, byte)[256];
    private static readonly TChar[][] HexSegments = new TChar[65536][];

    static IPv6AddressFormatter()
    {
        // Precompute Zero Sequences
        for (var m = 0; m < 256; ++m)
        {
            var mask = (byte)m;
            // Find longest 0 sequence
            byte zeroSequenceStart = 255;
            byte zeroSequenceLength = 1;
            byte i = 0;
            while (i < 8)
            {
                if ((mask & (byte)(1 << i)) != 0)
                {
                    ++i;
                    continue;
                }

                byte length = 1;
                for (var j = (byte)(i + 1); j < 8; ++j)
                {
                    if ((mask & (byte)(1 << j)) != 0)
                        break;
                    ++length;
                }

                if (length > zeroSequenceLength)
                {
                    zeroSequenceStart = i;
                    zeroSequenceLength = length;
                }

                i += (byte)(length + 1);
            }
            ZeroSequences[m] = (zeroSequenceStart, zeroSequenceLength);
        }
        // Precompute Hex Segments
        Span<TChar> buffer = stackalloc TChar[4];
        for (var s = 0; s < 65536; ++s)
        {
            var segment = (ushort)s;
            var digit1 = (byte)(segment >> 12);
            var digit2 = (byte)((segment >> 8) & 0xF);
            var digit3 = (byte)((segment >> 4) & 0xF);
            var digit4 = (byte)(segment & 0xF);

            var writer = new SpanWriter<TChar>(buffer);
            var written = false;

            if (digit1 != 0)
            {
                writer.TryWriteHexDigit(digit1);
                written = true;
            }

            if (digit2 != 0 || written)
            {
                writer.TryWriteHexDigit(digit2);
                written = true;
            }

            if (digit3 != 0 || written)
                writer.TryWriteHexDigit(digit3);

            writer.TryWriteHexDigit(digit4);

            HexSegments[s] = buffer[..writer.Position].ToArray();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<TChar> source, Span<byte> ipAddressBytes)
    {
        if (source.StartsWith(IPv4MappedToIPv6PrefixStr))
        {
            var ipv4Source = source[IPv4MappedToIPv6PrefixStr.Length..];
            if (ipv4Source.Contains(IPv4AddressFormatter<TChar>.Separator)) // IPv4 mapped to IPv6
            {
                IPv4MappedToIPv6Prefix.CopyTo(ipAddressBytes);
                return IPv4AddressFormatter<TChar>.TryParse(ipv4Source, ipAddressBytes[IPv4MappedToIPv6Prefix.Length..]);
            }
        }

        var reader = new SpanReader<TChar>(source);
        var ipAddressSegments = MemoryMarshal.Cast<byte, NetInt<ushort>>(ipAddressBytes);
        var count = 0;
        var zeroSequenceStart = -1;
        var maxSegmentCount = Unsafe.SizeOf<IPv6Address>() / 2;
        while (count < maxSegmentCount)
        {
            var separatorCount = 0;
            if (TryReadSeparator(ref reader))
            {
                ++separatorCount;
                if (TryReadSeparator(ref reader))
                    ++separatorCount;
            }

            switch (separatorCount)
            {
                case 1 when count == 0:
                case 2 when zeroSequenceStart >= 0:
                    return false;
                case 2:
                    zeroSequenceStart = count;
                    break;
            }

            if (!TryReadSegment(ref reader, out ipAddressSegments[count]))
            {
                if (separatorCount == 1)
                    return false;
                break;
            }

            ++count;
        }

        if (zeroSequenceStart < 0)
            return count == maxSegmentCount;

        var zeroSequenceLength = maxSegmentCount - count;
        if (zeroSequenceLength < 2)
            return false;

        var zeroSequenceEnd = zeroSequenceStart + zeroSequenceLength;
        ipAddressSegments[zeroSequenceStart..count].CopyTo(ipAddressSegments[zeroSequenceEnd..]);
        ipAddressSegments[zeroSequenceStart..zeroSequenceEnd].Clear();
        return reader.IsEndOfSource;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadSegment(ref SpanReader<TChar> reader, out NetInt<ushort> segment)
    {
        if (!reader.TryReadHexDigit(out var firstDigit))
        {
            Unsafe.SkipInit(out segment);
            return false;
        }

        int segmentInt = firstDigit;

        if (reader.TryReadHexDigit(out var secondDigit))
            segmentInt = (segmentInt << 4) + secondDigit;

        if (reader.TryReadHexDigit(out var thirdDigit))
            segmentInt = (segmentInt << 4) + thirdDigit;

        if (reader.TryReadHexDigit(out var fourthDigit))
            segmentInt = (segmentInt << 4) + fourthDigit;

        segment = (NetInt<ushort>)(ushort)segmentInt;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadSeparator(ref SpanReader<TChar> reader) => reader.TryReadOne(Separator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(ReadOnlySpan<byte> ipAddressBytes, bool isIPv4MappedToIPv6, Span<TChar> destination, out int charsWritten)
    {
        var writer = new SpanWriter<TChar>(destination);
        var result = isIPv4MappedToIPv6
            ? TryWriteIPv4MappedToIPv6(ref writer, ipAddressBytes)
            : TryWriteIPv6(ref writer, ipAddressBytes);
        charsWritten = writer.Position;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteIPv6(ref SpanWriter<TChar> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        var data = MemoryMarshal.Cast<byte, NetInt<ushort>>(ipAddressBytes);
        // Find longest 0 sequence
        byte mask = 0;
        for (byte i = 0; i < 8; ++i)
            if (data[i] != default)
                mask |= (byte)(1 << i);
        var (zeroSequenceStart, zeroSequenceLength) = ZeroSequences[mask];
        {
            var wasZeroSequence = false;
            byte i = 0;
            while (i < 8)
            {
                if (i == zeroSequenceStart)
                {
                    if (!writer.TryWrite(Separator) ||
                        !writer.TryWrite(Separator))
                        return false;

                    i += zeroSequenceLength;
                    wasZeroSequence = true;
                    continue;
                }

                if (wasZeroSequence)
                    wasZeroSequence = false;
                else if (i > 0 && !writer.TryWrite(Separator))
                    return false;

                if (!TryWriteSegment(ref writer, (ushort)data[i]))
                    return false;

                ++i;
            }
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteSegment(ref SpanWriter<TChar> writer, ushort segment) => writer.TryWrite(HexSegments[segment]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteIPv4MappedToIPv6(ref SpanWriter<TChar> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        return writer.TryWrite(IPv4MappedToIPv6PrefixStr) && IPv4AddressFormatter<TChar>.TryWrite(ref writer, ipAddressBytes[IPv4MappedToIPv6Prefix.Length..]);
    }
}