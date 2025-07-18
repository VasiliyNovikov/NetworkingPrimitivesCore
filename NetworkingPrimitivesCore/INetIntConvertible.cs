using System.Numerics;

namespace NetworkingPrimitivesCore;

public interface INetIntConvertible<T, TInt>
    where T : unmanaged, INetIntConvertible<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>
{
    static abstract explicit operator NetInt<TInt>(T value);
    static abstract explicit operator T(NetInt<TInt> value);
}