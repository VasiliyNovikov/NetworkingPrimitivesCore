using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public interface IIPNetworkBase<T, TAddress> : INetPrimitive<T>
    where T : unmanaged, IIPNetworkBase<T, TAddress>
    where TAddress : unmanaged, IIPAddressBase<TAddress>
{
    TAddress Address { get; }
    TAddress Mask { get; }
    byte Prefix { get; }
    TAddress Gateway { get; }
    TAddress Broadcast { get; }
    bool Contains(TAddress address);
    TAddress AddressAt<TIndex>(TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>;
    T Subnet<TIndex>(byte prefix, TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>;
    T Supernet(byte prefix);
}

public interface IIPNetwork<T, TAddress, TUInt> : IIPNetworkBase<T, TAddress>
    where T : unmanaged, IIPNetwork<T, TAddress, TUInt>
    where TAddress : unmanaged, IIPAddress<TAddress, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>
{
    static virtual int Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TAddress.Version;
    }
}