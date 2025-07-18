using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class IPv6AddressFormatter
{
    private const char Separator = ':';
    private const string IPv4MappedToIPv6PrefixStr = "::ffff:";
    private static readonly byte[] IPv4MappedToIPv6Prefix = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> source, Span<byte> ipAddressBytes)
    {
        if (source.StartsWith(IPv4MappedToIPv6PrefixStr))
        {
            var ipv4Source = source[IPv4MappedToIPv6PrefixStr.Length..];
            if (ipv4Source.Contains('.')) // IPv4 mapped to IPv6
            {
                IPv4MappedToIPv6Prefix.CopyTo(ipAddressBytes);
                return IPv4AddressFormatter.TryParse(ipv4Source, ipAddressBytes[IPv4MappedToIPv6Prefix.Length..]);
            }
        }

        var reader = new SpanReader<char>(source);
        Span<NetInt<ushort>> ipAddressSegments = MemoryMarshal.Cast<byte, NetInt<ushort>>(ipAddressBytes);
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

            if (separatorCount == 2)
            {
                if (zeroSequenceStart >= 0)
                    return false;
                zeroSequenceStart = count;
            }
            else if (separatorCount == 1 && count == 0)
                return false;


            if (!TryReadSegemnt(ref reader, out ipAddressSegments[count]))
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
    private static bool TryReadSegemnt(ref SpanReader<char> reader, out NetInt<ushort> segment)
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
    private static bool TryReadSeparator(ref SpanReader<char> reader) => reader.TryReadOne(Separator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(ReadOnlySpan<byte> ipAddressBytes, Span<char> destination, out int charsWritten)
    {
        var writer = new SpanWriter<char>(destination);
        var result = ipAddressBytes.StartsWith(IPv4MappedToIPv6Prefix)
            ? TryWriteIPv4MappedToIPv6(ref writer, ipAddressBytes)
            : TryWriteIPv6(ref writer, ipAddressBytes);
        charsWritten = writer.Length;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteIPv6(ref SpanWriter<char> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        var data = MemoryMarshal.Cast<byte, NetInt<ushort>>(ipAddressBytes);

        // Find longest 0 sequence
        var zeroSequenceStart = -1;
        var zeroSequenceLength = 1;
        var i = 0;
        while (i < data.Length)
        {
            if (data[i] != default)
            {
                ++i;
                continue;
            }

            var length = 1;
            for (var j = i + 1; j < data.Length; ++j)
            {
                if (data[j] != default)
                    break;
                ++length;
            }

            if (length > zeroSequenceLength)
            {
                zeroSequenceStart = i;
                zeroSequenceLength = length;
            }

            i += length + 1;
        }

        var wasZeroSequence = false;
        i = 0;
        while (i < data.Length)
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

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteSegment(ref SpanWriter<char> writer, ushort segment)
    {
        var digit1 = (byte)(segment >> 12);
        var digit2 = (byte)((segment >> 8) & 0xF);
        var digit3 = (byte)((segment >> 4) & 0xF);
        var digit4 = (byte)(segment & 0xF);

        var written = false;

        if (digit1 != 0)
            if (writer.TryWriteHexDigit(digit1))
                written = true;
            else
                return false;

        if (digit2 != 0 || written)
            if (writer.TryWriteHexDigit(digit2))
                written = true;
            else
                return false;

        if (digit3 != 0 || written)
            if (!writer.TryWriteHexDigit(digit3))
                return false;

        return writer.TryWriteHexDigit(digit4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteIPv4MappedToIPv6(ref SpanWriter<char> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        return writer.TryWrite(IPv4MappedToIPv6PrefixStr) && IPv4AddressFormatter.TryWrite(ref writer, ipAddressBytes[IPv4MappedToIPv6Prefix.Length..]);
    }
}