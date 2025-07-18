using System.Numerics;

namespace NetworkingPrimitivesCore.Json;

internal sealed class JsonNetAddressConverter<T, TInt> : SpanJsonConverter<T>
    where T : unmanaged, INetAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    protected override int MaxStringLength => T.MaxStringLength;
}
