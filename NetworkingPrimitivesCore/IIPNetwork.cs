using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public interface IIPNetworkBase<T, TAddress> : INetPrimitive<T>
    where T : unmanaged, IIPNetworkBase<T, TAddress>
    where TAddress : unmanaged, IIPAddressBase<TAddress>
{
    TAddress Address { get; }
    TAddress Mask { get; }
    byte Prefix { get; }
    TAddress Gateway { get; }
    TAddress Broadcast { get; }
    bool Contains(TAddress address);
    TAddress AddressAt<TIndex>(TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>;
    T Subnet<TIndex>(byte prefix, TIndex index) where TIndex : unmanaged, IBinaryInteger<TIndex>;
    T Supernet(byte prefix);

    static abstract bool TryParse<TChar>(ReadOnlySpan<TChar> source, bool strict, out T result) where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>;
    static abstract bool TryParse(string source, bool strict, out T result);
    static abstract T Parse<TChar>(ReadOnlySpan<TChar> source, bool strict) where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>;
    static abstract T Parse(string source, bool strict);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result)
    {
        return provider is IPNetworkFormatProvider formatProvider
            ? T.TryParse(s, formatProvider.IsStrict, out result)
            : T.TryParse(s, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IUtf8SpanParsable<T>.TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out T result)
    {
        return provider is IPNetworkFormatProvider formatProvider
            ? T.TryParse(utf8Text, formatProvider.IsStrict, out result)
            : T.TryParse(utf8Text, out result);
    }
}

public interface IIPNetwork<T, TAddress> : IIPNetworkBase<T, TAddress>
    where T : unmanaged, IIPNetwork<T, TAddress>
    where TAddress : unmanaged, IIPAddress<TAddress>
{
    static abstract byte Version { get; }
}

public interface IIPNetwork<T, TAddress, TUInt> : IIPNetwork<T, TAddress>
    where T : unmanaged, IIPNetwork<T, TAddress, TUInt>
    where TAddress : unmanaged, IIPAddress<TAddress, TUInt>
    where TUInt : unmanaged, IBinaryInteger<TUInt>, IUnsignedNumber<TUInt>;