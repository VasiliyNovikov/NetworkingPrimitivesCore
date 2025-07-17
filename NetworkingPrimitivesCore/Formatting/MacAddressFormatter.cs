using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class MacAddressFormatter<TChar>
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadRequiredSeparator(ref SpanReader<TChar> reader)
    {
        return reader.TryRead(out var ch) && (ch == TChar.CreateTruncating(':') || ch == TChar.CreateTruncating('-'));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadComponent(ref SpanReader<TChar> reader, out byte component)
    {
        if (!reader.TryReadHexDigit(out component))
            return false;

        if (reader.TryReadHexDigit(out var secondDigit))
            component = (byte)((component << 4) | secondDigit);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<TChar> source, Span<byte> macAddressBytes)
    {
        var reader = new SpanReader<TChar>(source);
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
    public static bool TryFormat(ReadOnlySpan<byte> macAddressBytes, Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format)
    {
        if (format.Length > 1)
            throw new FormatException($"The {format} format string is not supported");

        char formatChar = format.IsEmpty ? 'n' : format[0];
        TChar separator;
        bool isUpper;
        switch (formatChar)
        {
            case 'n':
                isUpper = false;
                separator = TChar.CreateTruncating(':');
                break;
            case 'N':
                isUpper = true;
                separator = TChar.CreateTruncating(':');
                break;
            case 'u':
                isUpper = false;
                separator = TChar.CreateTruncating('-');
                break;
            case 'U':
                isUpper = true;
                separator = TChar.CreateTruncating('-');
                break;
            default:
                charsWritten = 0;
                throw new FormatException($"The {format} format string is not supported.");
        }

        var writer = new SpanWriter<TChar>(destination);
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
        charsWritten = writer.Position;
        return result;
    }
}
