using System;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

internal sealed class IPNetworkFormatProvider : IFormatProvider
{
    private static readonly IPNetworkFormatProvider Strict = new(true);
    private static readonly IPNetworkFormatProvider NonStrict = new(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPNetworkFormatProvider Get(bool strict) => strict ? Strict : NonStrict;

    public bool IsStrict { get; }

    private IPNetworkFormatProvider(bool strict) => IsStrict = strict;

    public object? GetFormat(Type? formatType) => formatType == typeof(IPNetworkFormatProvider) ? this : null;
}