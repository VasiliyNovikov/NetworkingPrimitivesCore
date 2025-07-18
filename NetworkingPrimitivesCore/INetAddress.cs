using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore;

public interface INetAddress<T, TInt>
    : IEquatable<T>,
      IComparable<T>,
      IComparisonOperators<T, T, bool>,
      IBitwiseOperators<T, T, T>,
      ISpanParsable<T>,
      ISpanFormattable,
      INetIntConvertible<T, TInt>
    where T : unmanaged, INetAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    static abstract int MaxStringLength { get; }
    static virtual T Broadcast => (T)TInt.MaxValue;

    ReadOnlySpan<byte> Bytes { get; }

    bool TryFormat(Span<char> destination, out int charsWritten);

    static abstract bool TryParse(ReadOnlySpan<char> source, out T result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator TInt(T value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator T(TInt value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result) => T.TryParse(s, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<T>.TryParse(string? s, IFormatProvider? provider, out T result) => T.TryParse(s.AsSpan(), out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T ISpanParsable<T>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IParsable<T>.Parse(string s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
}