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
public class IPv6NetworkBenchmarks
{
    private static readonly string[] TestIPNetworkStrings =
    [
        "fec0::/64",
        "2a01:110:1008:b:45b1:911f:4b9a:48e7/128",
        "2001:db8::/32",
        "2001:db8::1/128"
    ];
    private static readonly string[] TestIPAddressStrings =
    [
        "fec0::1",
        "2a01:110:1008:b:45b1:911f:4b9a:48e7",
        "2001:db8::1234",
        "2001:db8:0:1::1"
    ];
    private static readonly IPNetwork[] TestIPNetworks = [.. TestIPNetworkStrings.Select(IPNetwork.Parse)];
    private static readonly IPv6Network[] TestIPv6Networks = [.. TestIPNetworkStrings.Select(IPv6Network.Parse)];
    private static readonly IPAddress[] TestIPAddresses = [.. TestIPAddressStrings.Select(IPAddress.Parse)];
    private static readonly IPv6Address[] TestIPv6Addresses = [.. TestIPAddressStrings.Select(IPv6Address.Parse)];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Parse")]
    public void Parse_IPNetwork()
    {
        foreach (var network in TestIPNetworkStrings)
            if (!IPNetwork.TryParse(network, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Parse")]
    public void Parse_IPv6Network()
    {
        foreach (var network in TestIPNetworkStrings)
            if (!IPv6Network.TryParse(network, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPNetwork()
    {
        Span<char> buffer = stackalloc char[IPv6Network.MaxStringLength];
        foreach (var network in TestIPNetworks)
            if (!network.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPv6Network()
    {
        Span<char> buffer = stackalloc char[IPv6Network.MaxStringLength];
        foreach (var network in TestIPv6Networks)
            if (!network.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Contains")]
    public int Contains_IPNetwork()
    {
        var count = 0;
        foreach (var network in TestIPNetworks)
        foreach (var address in TestIPAddresses)
            if (network.Contains(address))
                ++count;
        return count;
    }

    [Benchmark]
    [BenchmarkCategory("Contains")]
    public int Contains_IPv6Network()
    {
        var count = 0;
        foreach (var network in TestIPv6Networks)
        foreach (var address in TestIPv6Addresses)
            if (network.Contains(address))
                ++count;
        return count;
    }
}