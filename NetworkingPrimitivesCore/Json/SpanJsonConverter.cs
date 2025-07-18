using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore.Json;

internal abstract class SpanJsonConverter<T> : JsonConverter<T>
    where T : ISpanFormattable, ISpanParsable<T>
{
    protected abstract int MaxStringLength { get; }

    [SkipLocalsInit]
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string token but got {reader.TokenType}.");

        Span<char> buffer = stackalloc char[MaxStringLength];
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

    [SkipLocalsInit]
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[MaxStringLength];
        writer.WriteStringValue(buffer[..FormattingHelper.Format(value, buffer)]);
    }
}
