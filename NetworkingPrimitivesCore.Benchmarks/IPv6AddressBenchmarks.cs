using System;
using System.Linq;
using System.Net;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[HideColumns("Mean", "StdDev", "Error", "RatioSD", "Gen0", "Alloc Ratio")]
public class IPv6AddressBenchmarks
{
    private static readonly string[] TestIPv6AddressStrings =
    [
        "::",
        "::1",
        "fe80::1",
        "2001:db8::1",
        "2001:db8:0:1:1:1:1:1",
        "2001:db8:85a3::8a2e:370:7334",
        "0123:4567:89ab:cdef:0123:4567:89ab:cdef",
        "::ffff:192.168.0.1",
        "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"
    ];
    private static readonly IPAddress[] TestIPv6Addresses = [.. TestIPv6AddressStrings.Select(IPAddress.Parse)];
    private static readonly IPv6Address[] TestIPv6IPv6Addresses = [.. TestIPv6AddressStrings.Select(IPv6Address.Parse)];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Parse")]
    public void Parse_IPv6_IPAddress()
    {
        foreach (var address in TestIPv6AddressStrings)
            if (!IPAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Parse")]
    public void Parse_IPv6_IPv6Address()
    {
        foreach (var address in TestIPv6AddressStrings)
            if (!IPv6Address.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPv6_IPAddress()
    {
        Span<char> buffer = stackalloc char[IPv6Address.MaxStringLength];
        foreach (var address in TestIPv6Addresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPv6_IPv6Address()
    {
        Span<char> buffer = stackalloc char[IPv6Address.MaxStringLength];
        foreach (var address in TestIPv6IPv6Addresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}