namespace NetworkingPrimitivesCore.Converters;

internal sealed class NetAddressConverter<T> : SpanTypeConverter<T>
    where T : unmanaged, INetAddressBase<T>
{
    protected override int MaxStringLength => T.MaxStringLength;
}