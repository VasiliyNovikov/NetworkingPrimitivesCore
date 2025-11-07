using System;
using System.Buffers.Binary;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[HideColumns("Mean", "StdDev", "Error", "RatioSD", "Alloc Ratio")]
public class NetIntBenchmarks
{
    private const int TestCount = 1000;

    private static readonly ushort[] U16Values = [.. Enumerable.Range(0, TestCount).Select(_ => (ushort)Random.Shared.Next(ushort.MaxValue))];
    private static readonly uint[] U32Values = [.. Enumerable.Range(0, TestCount).Select(_ => (uint)Random.Shared.Next())];
    private static readonly ulong[] U64Values = [.. Enumerable.Range(0, TestCount).Select(_ => (ulong)Random.Shared.NextInt64())];
    private static readonly UInt128[] U128Values = [.. Enumerable.Range(0, TestCount).Select(_ => new UInt128((ulong)Random.Shared.NextInt64(), (ulong)Random.Shared.NextInt64()))];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("16")]
    public void U16_BinaryPrimitives()
    {
        foreach (var value in U16Values)
        {
            var reversed = BinaryPrimitives.ReverseEndianness(value);
            var original = BinaryPrimitives.ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    [BenchmarkCategory("16")]
    public void U16_NetInt()
    {
        foreach (var value in U16Values)
        {
            var reversed = (NetInt<ushort>)value;
            var original = (ushort)reversed;
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("32")]
    public void U32_BinaryPrimitives()
    {
        foreach (var value in U32Values)
        {
            var reversed = BinaryPrimitives.ReverseEndianness(value);
            var original = BinaryPrimitives.ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    [BenchmarkCategory("32")]
    public void U32_NetInt()
    {
        foreach (var value in U32Values)
        {
            var reversed = (NetInt<uint>)value;
            var original = (uint)reversed;
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("64")]
    public void U64_BinaryPrimitives()
    {
        foreach (var value in U64Values)
        {
            var reversed = BinaryPrimitives.ReverseEndianness(value);
            var original = BinaryPrimitives.ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    [BenchmarkCategory("64")]
    public void U64_NetInt()
    {
        foreach (var value in U64Values)
        {
            var reversed = (NetInt<ulong>)value;
            var original = (ulong)reversed;
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("128")]
    public void U128_BinaryPrimitives()
    {
        foreach (var value in U128Values)
        {
            var reversed = BinaryPrimitives.ReverseEndianness(value);
            var original = BinaryPrimitives.ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    [BenchmarkCategory("128")]
    public void U128_NetInt()
    {
        foreach (var value in U128Values)
        {
            var reversed = (NetInt<UInt128>)value;
            var original = (UInt128)reversed;
            if (value != original)
                throw new InvalidOperationException();
        }
    }
}