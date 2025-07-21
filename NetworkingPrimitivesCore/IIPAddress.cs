using System.Numerics;

namespace NetworkingPrimitivesCore;

public interface IIPAddressBase<T> : INetAddressBase<T>
    where T : unmanaged, IIPAddressBase<T>
{
    bool IsLinkLocal { get; }
}

public interface IIPAddress<T, TUInt> : INetAddress<T, TUInt>
    where T : unmanaged, IIPAddress<T, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>, IMinMaxValue<TUInt>
{
    static abstract T Any { get; }
    static abstract T Loopback { get; }
    static abstract int Version { get; }
    bool IsLinkLocal { get; }
}