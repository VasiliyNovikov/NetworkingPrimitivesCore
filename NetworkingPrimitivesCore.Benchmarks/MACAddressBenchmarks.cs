using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class MACAddressBenchmarks
{
    private static readonly string[] TestMACAddressStrings =
    [
        "00:00:00:00:00:00",
        "ff:ff:ff:ff:ff:ff",
        "01:02:03:04:05:06",
        "a0:b0:c0:d0:e0:f0",
        "1:2:3:4:5:6"
    ];

    private static readonly MACAddress[] TestMACAddresses = [.. TestMACAddressStrings.Select(MACAddress.Parse)];

    [Benchmark]
    public void Parse_MACAddress()
    {
        foreach (var address in TestMACAddressStrings)
            if (!MACAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    public void Format_MACAddress()
    {
        Span<char> buffer = stackalloc char[MACAddress.MaxStringLength];
        foreach (var address in TestMACAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}