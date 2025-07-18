using System;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class MacAddressFormatter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadRequiredSeparator(ref SpanReader<char> reader) => reader.TryRead(out var ch) && ch is ':' or '-';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadComponent(ref SpanReader<char> reader, out byte component)
    {
        if (!reader.TryReadHexDigit(out component))
            return false;

        if (reader.TryReadHexDigit(out var secondDigit))
            component = (byte)((component << 4) | secondDigit);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> source, Span<byte> macAddressBytes)
    {
        var reader = new SpanReader<char>(source);
        for (var i = 0; i < macAddressBytes.Length; ++i)
        {
            if (i > 0 && !TryReadRequiredSeparator(ref reader))
                return false;

            if (!TryReadComponent(ref reader, out macAddressBytes[i]))
                return false;
        }
        return reader.Position == source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(ReadOnlySpan<byte> macAddressBytes, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
    {
        if (format.Length > 1)
            throw new FormatException($"The {format} format string is not supported");

        if (format.IsEmpty)
            format = "n";

        char separator;
        bool isUpper;
        switch (format[0])
        {
            case 'n':
                isUpper = false;
                separator = ':';
                break;
            case 'N':
                isUpper = true;
                separator = ':';
                break;
            case 'u':
                isUpper = false;
                separator = '-';
                break;
            case 'U':
                isUpper = true;
                separator = '-';
                break;
            default:
                charsWritten = default;
                throw new FormatException($"The {format} format string is not supported.");
        }

        var writer = new SpanWriter<char>(destination);
        bool result = true;
        for (var i = 0; i < macAddressBytes.Length; ++i)
        {
            var component = macAddressBytes[i];
            if ((i == 0 || writer.TryWrite(separator)) &&
                writer.TryWriteHexDigit((byte)(component >> 4), isUpper) &&
                writer.TryWriteHexDigit((byte)(component & 0xF), isUpper))
                continue;
            result = false;
            break;
        }
        charsWritten = writer.Length;
        return result;
    }
}
