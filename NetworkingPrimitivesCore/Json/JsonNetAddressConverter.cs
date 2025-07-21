namespace NetworkingPrimitivesCore.Json;

internal sealed class JsonNetAddressConverter<T> : SpanJsonConverter<T>
    where T : unmanaged, INetAddressBase<T>
{
    protected override int MaxStringLength => T.MaxStringLength;
}
