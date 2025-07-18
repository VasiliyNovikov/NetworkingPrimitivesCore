using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class MacAddressBenchmarks
{
    private static readonly string[] TestMacAddressStrings =
    [
        "00:00:00:00:00:00",
        "ff:ff:ff:ff:ff:ff",
        "01:02:03:04:05:06",
        "a0:b0:c0:d0:e0:f0",
        "1:2:3:4:5:6"
    ];

    private static readonly MacAddress[] TestMacAddresses = [.. TestMacAddressStrings.Select(MacAddress.Parse)];

    [Benchmark]
    public void Parse_MacAddress()
    {
        foreach (var address in TestMacAddressStrings)
            if (!MacAddress.TryParse(address, out _))
                throw new InvalidOperationException();
    }

    [Benchmark]
    public void Format_MacAddress()
    {
        Span<char> buffer = stackalloc char[MacAddress.MaxStringLength];
        foreach (var address in TestMacAddresses)
            if (!address.TryFormat(buffer, out _))
                throw new InvalidOperationException();
    }
}