using System;
using System.Linq;
using System.Net.NetworkInformation;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[HideColumns("Mean", "StdDev", "Error", "RatioSD", "Gen0", "Alloc Ratio")]
public class MACAddressBenchmarks
{
    private static readonly string[] TestMACAddressStrings =
    [
        "00:00:00:00:00:00",
        "ff:ff:ff:ff:ff:ff",
        "01:02:03:04:05:06",
        "a0:b0:c0:d0:e0:f0",
        "01:02:03:04:05:06"
    ];

    private static readonly MACAddress[] TestMACAddresses = [.. TestMACAddressStrings.Select(MACAddress.Parse)];

    private static readonly PhysicalAddress[] TestPhysicalAddresses = [.. TestMACAddressStrings.Select(PhysicalAddress.Parse)];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Parse")]
    public void Parse_PhysicalAddress()
    {
        foreach (var address in TestMACAddressStrings)
            if (!PhysicalAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    [BenchmarkCategory("Parse")]
    public void Parse_MACAddress()
    {
        foreach (var address in TestMACAddressStrings)
            if (!MACAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Format")]
    public string Format_PhysicalAddress()
    {
        var result = string.Empty;
        foreach (var address in TestPhysicalAddresses)
            result = address.ToString();
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Format")]
    public void Format_MACAddress()
    {
        Span<char> buffer = stackalloc char[MACAddress.MaxStringLength];
        foreach (var address in TestMACAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}