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
public class IPAnyAddressBenchmarks
{
    private static readonly string[] TestIPAddressStrings =
    [
        "127.0.0.1",
        "192.168.0.1",
        "10.10.128.13",
        "1.2.3.4",
        "123.245.156.78",
        "255.255.255.255",
        "0.0.0.0",
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
    private static readonly IPAddress[] TestIPAddresses = [.. TestIPAddressStrings.Select(IPAddress.Parse)];
    private static readonly IPAnyAddress[] TestIPAnyAddresses = [.. TestIPAddressStrings.Select(IPAnyAddress.Parse)];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Parse")]
    public void Parse_IPAddress()
    {
        foreach (var address in TestIPAddressStrings)
            if (!IPAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Parse")]
    public void Parse_IPAnyAddress()
    {
        foreach (var address in TestIPAddressStrings)
            if (!IPAnyAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public void Format_IPAddress()
    {
        Span<char> buffer = stackalloc char[IPAnyAddress.MaxStringLength];
        foreach (var address in TestIPAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_IPAnyAddress()
    {
        Span<char> buffer = stackalloc char[IPAnyAddress.MaxStringLength];
        foreach (var address in TestIPAnyAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}
