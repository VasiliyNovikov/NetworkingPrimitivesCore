using System;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

public static class FormattingHelper
{
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(this T value, int maxStringLength, string? format = null, IFormatProvider? provider = null) where T : ISpanFormattable
    {
        Span<char> buffer = stackalloc char[maxStringLength];
        return buffer[..Format(value, buffer, format, provider)].ToString();
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Format<T>(this T value, Span<char> destination, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        where T : ISpanFormattable
    {
        return value.TryFormat(destination, out int charsWritten, format, provider)
            ? charsWritten
            : throw new FormatException($"The value of type {typeof(T)} could not be formatted with the format '{format}' and provider '{provider}'.");
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Parse<T>(ReadOnlySpan<char> source, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        return T.TryParse(source, provider, out var result)
            ? result
            : throw new FormatException($"The string '{source}' could not be parsed into a value of type {typeof(T)}.");
    }
}