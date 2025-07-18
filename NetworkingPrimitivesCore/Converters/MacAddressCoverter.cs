namespace NetworkingPrimitivesCore.Converters;

public sealed class MacAddressCoverter : SpanTypeConverter<MacAddress>
{
    protected override int MaxStringLength => MacAddress.StringLength;
}