using System;
using System.Buffers.Binary;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

namespace NetworkingPrimitivesCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class ReverseEndiannessBenchmarks
{
    private const int TestCount = 1000;

    private static readonly ushort[] U16Values = [.. Enumerable.Range(0, TestCount).Select(_ => (ushort)Random.Shared.Next(ushort.MaxValue))];
    private static readonly uint[] U32Values = [.. Enumerable.Range(0, TestCount).Select(_ => (uint)Random.Shared.Next())];
    private static readonly ulong[] U64Values = [.. Enumerable.Range(0, TestCount).Select(_ => (ulong)Random.Shared.NextInt64())];
    private static readonly UInt128[] U128Values = [.. Enumerable.Range(0, TestCount).Select(_ => new UInt128((ulong)Random.Shared.NextInt64(), (ulong)Random.Shared.NextInt64()))];

    [Benchmark]
    public void ReverseEndianness_U16_Direct()
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
    public void ReverseEndianness_U16_Generic()
    {
        foreach (var value in U16Values)
        {
            var reversed = ReverseEndianness(value);
            var original = ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    public void ReverseEndianness_U32_Direct()
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
    public void ReverseEndianness_U32_Generic()
    {
        foreach (var value in U32Values)
        {
            var reversed = ReverseEndianness(value);
            var original = ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    public void ReverseEndianness_U64_Direct()
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
    public void ReverseEndianness_U64_Generic()
    {
        foreach (var value in U64Values)
        {
            var reversed = ReverseEndianness(value);
            var original = ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [Benchmark]
    public void ReverseEndianness_U128_Direct()
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
    public void ReverseEndianness_U128_Generic()
    {
        foreach (var value in U128Values)
        {
            var reversed = ReverseEndianness(value);
            var original = ReverseEndianness(reversed);
            if (value != original)
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReverseEndianness<T>(T value) where T : unmanaged, IBinaryInteger<T> => BinaryPrimitives.ReverseEndianness(value);
}