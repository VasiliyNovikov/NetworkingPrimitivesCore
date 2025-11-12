using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public interface INetAddressBase<T> : INetPrimitive<T>
    where T : unmanaged, INetAddressBase<T>
{
    ReadOnlySpan<byte> Bytes { get; }
}

public interface INetAddress<T> : INetAddressBase<T>, IBitwiseOperators<T, T, T>
    where T : unmanaged, INetAddress<T>
{
    static abstract T Broadcast { get; }
}

public interface INetAddress<T, TUInt> : INetAddress<T>, INetIntConvertible<T, TUInt>
    where T : unmanaged, INetAddress<T, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator TUInt(T value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract explicit operator T(TUInt value);
}