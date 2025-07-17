using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NetworkingPrimitivesCore.Formatting;

namespace NetworkingPrimitivesCore.Converters;

internal sealed class NetPrimitiveTypeConverter<T> : TypeConverter
    where T : unmanaged, INetPrimitive<T>
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value is string str
            ? FormattingHelper.Parse<T, char>(str, culture)
            : base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return destinationType == typeof(string) && value is T typedValue
            ? typedValue.ToString(T.MaxStringLength, null, culture)
            : base.ConvertTo(context, culture, value, destinationType);
    }
}