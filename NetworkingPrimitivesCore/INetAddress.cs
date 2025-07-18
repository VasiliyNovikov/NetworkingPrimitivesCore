using System;
using System.Numerics;

namespace NetworkingPrimitivesCore;

public interface INetAddress<T, TInt>
    : IEquatable<T>,
      IComparable<T>,
      IComparisonOperators<T, T, bool>,
      ISpanParsable<T>,
      ISpanFormattable
    where T : unmanaged, INetAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    ReadOnlySpan<byte> Bytes { get; }

    bool TryFormat(Span<char> destination, out int charsWritten);

    static virtual T Broadcast => (T)TInt.MaxValue;

    static abstract bool TryParse(ReadOnlySpan<char> source, out T result);

    static virtual explicit operator TInt(T value) => (TInt)(NetInt<TInt>)value;
    static virtual explicit operator T(TInt value) => (T)(NetInt<TInt>)value;

    static abstract explicit operator NetInt<TInt>(T value);
    static abstract explicit operator T(NetInt<TInt> value);
}