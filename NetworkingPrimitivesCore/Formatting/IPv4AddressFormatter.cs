using System;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

internal static class IPv4AddressFormatter
{
    private const char Separator = '.';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> source, Span<byte> ipAddressBytes)
    {
        var reader = new SpanReader<char>(source);
        for (var i = 0; i < Unsafe.SizeOf<IPv4Address>(); ++i)
        {
            if (i > 0 && !TryReadRequiredSeparator(ref reader))
                return false;

            if (!TryReadComponent(ref reader, out ipAddressBytes[i]))
                return false;
        }
        return reader.IsEndOfSource;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadComponent(ref SpanReader<char> reader, out byte component)
    {
        if (!reader.TryReadDecimalDigit(out var firstDigit))
        {
            Unsafe.SkipInit(out component);
            return false;
        }

        int componentInt = firstDigit;

        if (reader.TryReadDecimalDigit(out var secondDigit))
            componentInt = (componentInt * 10) + secondDigit;

        if (reader.TryReadDecimalDigit(out var thirdDigit))
            componentInt = (componentInt * 10) + thirdDigit;

        if (componentInt > Byte.MaxValue)
        {
            Unsafe.SkipInit(out component);
            return false;
        }

        component = (byte)componentInt;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadRequiredSeparator(ref SpanReader<char> reader) => reader.TryRead(out var ch) && ch == Separator;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFormat(ReadOnlySpan<byte> ipAddressBytes, Span<char> destination, out int charsWritten)
    {
        var writer = new SpanWriter<char>(destination);
        var result = TryWrite(ref writer, ipAddressBytes);
        charsWritten = writer.Length;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryWrite(ref SpanWriter<char> writer, ReadOnlySpan<byte> ipAddressBytes)
    {
        for (var i = 0; i < Unsafe.SizeOf<IPv4Address>(); ++i)
        {
            if (i > 0 && !writer.TryWrite(Separator))
                return false;

            var (firstAndSecondDigits, thirdDigit) = Math.DivRem(ipAddressBytes[i], (byte)10);
            var (firstDigit, secondDigit) = Math.DivRem(firstAndSecondDigits, (byte)10);

            if (firstDigit != 0 && !writer.TryWriteDecimalDigit(firstDigit))
                return false;

            if ((secondDigit != 0 || firstDigit != 0) && !writer.TryWriteDecimalDigit(secondDigit))
                return false;

            if (!writer.TryWriteDecimalDigit(thirdDigit))
                return false;
        }
        return true;
    }
}