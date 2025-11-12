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
public class IPv4NetworkBenchmarks
{
    private static readonly string[] TestIPNetworkStrings =
    [
        "10.10.128.0/22",
        "10.10.128.0/21",
        "10.10.128.0/20",
        "10.10.128.3/32"
    ];
    private static readonly string[] TestIPAddressStrings =
    [
        "1.2.3.4",
        "10.10.131.2",
        "11.22.33.44",
        "10.10.128.1",
    ];
    private static readonly IPNetwork[] TestIPNetworks = [.. TestIPNetworkStrings.Select(IPNetwork.Parse)];
    private static readonly IPv4Network[] TestIPv4Networks = [.. TestIPNetworkStrings.Select(IPv4Network.Parse)];
    private static readonly IPAddress[] TestIPAddresses = [.. TestIPAddressStrings.Select(IPAddress.Parse)];
    private static readonly IPv4Address[] TestIPv4Addresses = [.. TestIPAddressStrings.Select(IPv4Address.Parse)];

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
    public void Parse_IPv4Network()
    {
        foreach (var network in TestIPNetworkStrings)
            if (!IPv4Network.TryParse(network, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPNetwork()
    {
        Span<char> buffer = stackalloc char[IPv4Network.MaxStringLength];
        foreach (var network in TestIPNetworks)
            if (!network.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPv4Network()
    {
        Span<char> buffer = stackalloc char[IPv4Network.MaxStringLength];
        foreach (var network in TestIPv4Networks)
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
    public int Contains_IPv4Network()
    {
        var count = 0;
        foreach (var network in TestIPv4Networks)
            foreach (var address in TestIPv4Addresses)
                if (network.Contains(address))
                    ++count;
        return count;
    }
}