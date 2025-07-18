using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore.Json;

public sealed class JsonMacAddressConverter : SpanJsonConverter<MacAddress>
{
    protected override int MaxStringLength => MacAddressFormatter.StringLength;
}
