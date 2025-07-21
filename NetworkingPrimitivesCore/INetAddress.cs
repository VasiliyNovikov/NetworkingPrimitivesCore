using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore;

public interface INetAddressBase<T>
    : IEquatable<T>
    , IComparable<T>
    , IComparisonOperators<T, T, bool>
    , ISpanParsable<T>
    , ISpanFormattable
    where T : unmanaged, INetAddressBase<T>
{
    static abstract int MaxStringLength { get; }

    ReadOnlySpan<byte> Bytes { get; }

    bool TryFormat(Span<char> destination, out int charsWritten);

    static abstract bool TryParse(ReadOnlySpan<char> source, out T result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result) => T.TryParse(s, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<T>.TryParse(string? s, IFormatProvider? provider, out T result) => T.TryParse(s.AsSpan(), out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T ISpanParsable<T>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T IParsable<T>.Parse(string s, IFormatProvider? provider) => FormattingHelper.Parse<T>(s, provider);
}

public interface INetAddress<T, TUInt>
    : INetAddressBase<T>
    , IBitwiseOperators<T, T, T>
    , INetIntConvertible<T, TUInt>
    where T : unmanaged, INetAddress<T, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>, IMinMaxValue<TUInt>
{
    static virtual T Broadcast => (T)TUInt.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator TUInt(T value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator T(TUInt value);
}