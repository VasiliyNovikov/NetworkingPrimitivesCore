using System.Numerics;

namespace NetworkingPrimitivesCore;

public interface IIPAddressBase<T> : INetAddressBase<T>
    where T : unmanaged, IIPAddressBase<T>
{
    bool IsLinkLocal { get; }
}

public interface IIPAddress<T> : IIPAddressBase<T>, INetAddress<T>
    where T : unmanaged, IIPAddress<T>
{
    static abstract T Any { get; }
    static abstract T Loopback { get; }
    static abstract byte Version { get; }
}

public interface IIPAddress<T, TUInt> : IIPAddress<T>, INetAddress<T, TUInt>
    where T : unmanaged, IIPAddress<T, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>;