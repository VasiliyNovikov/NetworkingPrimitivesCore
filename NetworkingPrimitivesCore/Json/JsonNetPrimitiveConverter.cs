using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore.Json;

internal sealed class JsonNetPrimitiveConverter<T> : JsonConverter<T>
    where T : unmanaged, INetPrimitive<T>
{
    [SkipLocalsInit]
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string token but got {reader.TokenType}.");

        Span<char> buffer = stackalloc char[T.MaxStringLength];
        buffer = buffer[..reader.CopyString(buffer)];
        try
        {
            return FormattingHelper.Parse<T>(buffer);
        }
        catch (FormatException e)
        {
            throw new JsonException($"Failed to parse '{buffer.ToString()}' into {typeof(T)}.", e);
        }
    }

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    [SkipLocalsInit]
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[T.MaxStringLength];
        writer.WriteStringValue(buffer[..FormattingHelper.Format(value, buffer)]);
    }

    [SkipLocalsInit]
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[T.MaxStringLength];
        writer.WritePropertyName(buffer[..FormattingHelper.Format(value, buffer)]);
    }
}
