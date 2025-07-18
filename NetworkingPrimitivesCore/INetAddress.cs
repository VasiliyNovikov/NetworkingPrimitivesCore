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
      ISpanFormattable
    where T : unmanaged, INetAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    static abstract int MaxStringLength { get; }
    ReadOnlySpan<byte> Bytes { get; }

    bool TryFormat(Span<char> destination, out int charsWritten);

    static virtual T Broadcast => (T)TInt.MaxValue;

    static abstract bool TryParse(ReadOnlySpan<char> source, out T result);

    static abstract explicit operator NetInt<TInt>(T value);
    static abstract explicit operator T(NetInt<TInt> value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static virtual explicit operator TInt(T value) => (TInt)(NetInt<TInt>)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static virtual explicit operator T(TInt value) => (T)(NetInt<TInt>)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<T, T, bool>.operator ==(T a, T b) => (NetInt<TInt>)a == (NetInt<TInt>)b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<T, T, bool>.operator !=(T a, T b) => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<T, T, bool>.operator <(T a, T b) => (TInt)a < (TInt)b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<T, T, bool>.operator >(T a, T b) => (TInt)a > (TInt)b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<T, T, bool>.operator <=(T a, T b) => (TInt)a <= (TInt)b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<T, T, bool>.operator >=(T a, T b) => (TInt)a >= (TInt)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IBitwiseOperators<T, T, T>.operator ~(T value) => (T)~(NetInt<TInt>)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IBitwiseOperators<T, T, T>.operator &(T a, T b) => (T)((NetInt<TInt>)a & (NetInt<TInt>)b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IBitwiseOperators<T, T, T>.operator |(T a, T b) => (T)((NetInt<TInt>)a | (NetInt<TInt>)b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IBitwiseOperators<T, T, T>.operator ^(T a, T b) => (T)((NetInt<TInt>)a ^ (NetInt<TInt>)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result) => T.TryParse(s, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<T>.TryParse(string? s, IFormatProvider? provider, out T result) => T.TryParse(s.AsSpan(), out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T ISpanParsable<T>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IParsable<T>.Parse(string s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
}