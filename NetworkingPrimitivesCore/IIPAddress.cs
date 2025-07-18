using System.Numerics;

namespace NetworkingPrimitivesCore;

public interface IIPAddress<T, TInt> : INetAddress<T, TInt>
    where T : unmanaged, IIPAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    static abstract T Any { get; }
    static abstract T Loopback { get; }
    static abstract int Version { get; }
    bool IsLinkLocal { get; }
}