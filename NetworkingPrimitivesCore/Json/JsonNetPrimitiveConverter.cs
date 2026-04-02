using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore.Json;

internal sealed class JsonNetPrimitiveConverter<T> : JsonConverter<T>
    where T : unmanaged, INetPrimitive<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.String
            ? ReadCore(ref reader)
            : throw new JsonException($"Expected string token but got {reader.TokenType}.");
    }

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.PropertyName
            ? ReadCore(ref reader)
            : throw new JsonException($"Expected property name token but got {reader.TokenType}.");
    }

    [SkipLocalsInit]
    private static T ReadCore(ref Utf8JsonReader reader)
    {
        Span<byte> buffer = stackalloc byte[T.MaxStringLength];
        buffer = buffer[..reader.CopyString(buffer)];
        try
        {
            return FormattingHelper.Parse<T, byte>(buffer);
        }
        catch (FormatException e)
        {
            throw new JsonException(e.Message, e);
        }
    }

    [SkipLocalsInit]
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Span<byte> buffer = stackalloc byte[T.MaxStringLength];
        writer.WriteStringValue(buffer[..value.Format(buffer)]);
    }

    [SkipLocalsInit]
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Span<byte> buffer = stackalloc byte[T.MaxStringLength];
        writer.WritePropertyName(buffer[..value.Format(buffer)]);
    }
}
