using System;
using System.Linq;
using System.Net;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class IPv4AddressBenchmarks
{
    private static readonly string[] TestIPv4AddressStrings =
    [
        "127.0.0.1",
        "192.168.0.1",
        "10.10.128.13",
        "1.2.3.4",
        "123.245.156.78",
        "255.255.255.255",
        "0.0.0.0"
    ];
    private static readonly IPAddress[] TestIPv4FrameworkAddresses = [.. TestIPv4AddressStrings.Select(IPAddress.Parse)];
    private static readonly IPv4Address[] TestIPv4IPv4Addresses = [.. TestIPv4AddressStrings.Select(IPv4Address.Parse)];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Parse")]
    public void Parse_IPv4_IPAddress()
    {
        foreach (var address in TestIPv4AddressStrings)
            if (!IPAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Parse")]
    public void Parse_IPv4_IPv4Address()
    {
        foreach (var address in TestIPv4AddressStrings)
            if (!IPv4Address.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPv4_IPAddress()
    {
        Span<char> buffer = stackalloc char[IPv4Address.MaxStringLength];
        foreach (var address in TestIPv4FrameworkAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPv4_IPv4Address()
    {
        Span<char> buffer = stackalloc char[IPv4Address.MaxStringLength];
        foreach (var address in TestIPv4IPv4Addresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}
