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
public class IPAnyNetworkBenchmarks
{
    private static readonly string[] TestIPNetworkStrings =
    [
        "10.10.128.0/22",
        "10.10.128.0/21",
        "10.10.128.0/20",
        "10.10.128.3/32",
        "fec0::/64",
        "2a01:110:1008:b:45b1:911f:4b9a:48e7/128",
        "2001:db8::/32",
        "2001:db8::1/128"
    ];
    private static readonly string[] TestIPAddressStrings =
    [
        "1.2.3.4",
        "10.10.131.2",
        "11.22.33.44",
        "10.10.128.1",
        "fec0::1",
        "2a01:110:1008:b:45b1:911f:4b9a:48e7",
        "2001:db8::1234",
        "2001:db8:0:1::1"
    ];
    private static readonly IPNetwork[] TestIPNetworks = [.. TestIPNetworkStrings.Select(IPNetwork.Parse)];
    private static readonly IPAnyNetwork[] TestIPAnyNetworks = [.. TestIPNetworkStrings.Select(IPAnyNetwork.Parse)];
    private static readonly IPAddress[] TestIPAddresses = [.. TestIPAddressStrings.Select(IPAddress.Parse)];
    private static readonly IPAnyAddress[] TestIPAnyAddresses = [.. TestIPAddressStrings.Select(IPAnyAddress.Parse)];

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
    public void Parse_IPAnyNetwork()
    {
        foreach (var network in TestIPNetworkStrings)
            if (!IPAnyNetwork.TryParse(network, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPNetwork()
    {
        Span<char> buffer = stackalloc char[IPAnyNetwork.MaxStringLength];
        foreach (var network in TestIPNetworks)
            if (!network.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPAnyNetwork()
    {
        Span<char> buffer = stackalloc char[IPAnyNetwork.MaxStringLength];
        foreach (var network in TestIPAnyNetworks)
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
    public int Contains_IPAnyNetwork()
    {
        var count = 0;
        foreach (var network in TestIPAnyNetworks)
            foreach (var address in TestIPAnyAddresses)
                if (network.Contains(address))
                    ++count;
        return count;
    }
}