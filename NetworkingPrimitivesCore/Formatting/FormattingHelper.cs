using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkingPrimitivesCore.Formatting;

internal static class FormattingHelper
{
    public static bool TryFormat<T, TChar>(this T value, Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        where T : unmanaged, ISpanFormattable, IUtf8SpanFormattable
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return typeof(TChar) == typeof(char)
            ? value.TryFormat(MemoryMarshal.Cast<TChar, char>(destination), out charsWritten, format, provider)
            : typeof(TChar) == typeof(byte)
                ? value.TryFormat(MemoryMarshal.Cast<TChar, byte>(destination), out charsWritten, format, provider)
                : throw new NotSupportedException($"The character type '{typeof(TChar)}' is not supported. Only 'char' and 'byte' are supported.");
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(this T value, int maxStringLength, string? format = null, IFormatProvider? provider = null)
        where T : unmanaged, ISpanFormattable, IUtf8SpanFormattable
    {
        Span<char> buffer = stackalloc char[maxStringLength];
        return buffer[..Format(value, buffer, format, provider)].ToString();
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Format<T, TChar>(this T value, Span<TChar> destination, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        where T : unmanaged, ISpanFormattable, IUtf8SpanFormattable
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return value.TryFormat(destination, out var charsWritten, format, provider)
            ? charsWritten
            : throw new FormatException($"The value of type {typeof(T)} could not be formatted with the format '{format}' and provider '{provider}'.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T, TChar>(ReadOnlySpan<TChar> source, IFormatProvider? provider, out T result)
        where T : unmanaged, ISpanParsable<T>, IUtf8SpanParsable<T>
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        return typeof(TChar) == typeof(char)
            ? T.TryParse(MemoryMarshal.Cast<TChar, char>(source), provider, out result)
            : typeof(TChar) == typeof(byte)
                ? T.TryParse(MemoryMarshal.Cast<TChar, byte>(source), provider, out result)
                : throw new NotSupportedException($"The character type '{typeof(TChar)}' is not supported. Only 'char' and 'byte' are supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Parse<T, TChar>(ReadOnlySpan<TChar> source, IFormatProvider? provider = null)
        where T : unmanaged, ISpanParsable<T>, IUtf8SpanParsable<T>
        where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
    {
        if (TryParse<T, TChar>(source, provider, out var result))
            return result;

        var sourceString = typeof(TChar) == typeof(char)
            ? source.ToString()
            : Encoding.UTF8.GetString(MemoryMarshal.Cast<TChar, byte>(source));
        throw new FormatException($"The string '{sourceString}' could not be parsed into a value of type {typeof(T)}.");
    }
}