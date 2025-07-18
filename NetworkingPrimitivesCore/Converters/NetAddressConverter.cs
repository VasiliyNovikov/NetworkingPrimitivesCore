using System.Numerics;

namespace NetworkingPrimitivesCore.Converters;

internal sealed class NetAddressConverter<T, TInt> : SpanTypeConverter<T>
    where T : unmanaged, INetAddress<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>, IUnsignedNumber<TInt>, IMinMaxValue<TInt>
{
    protected override int MaxStringLength => T.MaxStringLength;
}