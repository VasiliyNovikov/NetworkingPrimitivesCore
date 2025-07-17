using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore;

public interface INetPrimitive<T>
    : IEquatable<T>
    , IComparable<T>
    , IComparisonOperators<T, T, bool>
    , ISpanParsable<T>
    , ISpanFormattable
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<T>
    where T : unmanaged, INetPrimitive<T>
{
    static abstract int MaxStringLength { get; }

    bool TryFormat<TChar>(Span<TChar> destination, out int charsWritten) where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>;

    static abstract bool TryParse<TChar>(ReadOnlySpan<TChar> source, out T result) where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result) => T.TryParse(s, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IUtf8SpanParsable<T>.TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out T result) => T.TryParse(utf8Text, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<T>.TryParse(string? s, IFormatProvider? provider, out T result) => T.TryParse(s.AsSpan(), out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T ISpanParsable<T>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FormattingHelper.Parse<T, char>(s, provider);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IUtf8SpanParsable<T>.Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => FormattingHelper.Parse<T, byte>(utf8Text, provider);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IParsable<T>.Parse(string s, IFormatProvider? provider) => FormattingHelper.Parse<T, char>(s, provider);
}