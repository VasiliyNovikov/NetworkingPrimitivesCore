using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore;

[StructLayout(LayoutKind.Sequential)]
public readonly struct NetInt<T>
    : IEquatable<NetInt<T>>,
      IComparable<NetInt<T>>,
      IComparisonOperators<NetInt<T>, NetInt<T>, bool>,
      IBitwiseOperators<NetInt<T>, NetInt<T>, NetInt<T>>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly T _value;

    private T ConvertedValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Convert(_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NetInt(T value) => _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(NetInt<T> other) => _value == other._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NetInt<T> other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(NetInt<T> other) => ConvertedValue.CompareTo(other.ConvertedValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NetInt<T> left, NetInt<T> right) => left._value == right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NetInt<T> left, NetInt<T> right) => left._value != right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(NetInt<T> left, NetInt<T> right) => left.ConvertedValue < right.ConvertedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(NetInt<T> left, NetInt<T> right) => left.ConvertedValue <= right.ConvertedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(NetInt<T> left, NetInt<T> right) => left.ConvertedValue > right.ConvertedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(NetInt<T> left, NetInt<T> right) => left.ConvertedValue >= right.ConvertedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetInt<T> operator &(NetInt<T> left, NetInt<T> right) => new(left._value & right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetInt<T> operator |(NetInt<T> left, NetInt<T> right) => new(left._value | right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetInt<T> operator ^(NetInt<T> left, NetInt<T> right) => new(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetInt<T> operator ~(NetInt<T> value) => new(~value._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator T(NetInt<T> value) => value.ConvertedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator NetInt<T>(T value) => new(Convert(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // It was benchmarked in ReverseEndiannessBenchmarks and showed
    // to have the same performance as direct call of BinaryPrimitives.ReverseEndianness for specific types
    // and to not do any memory allocations
    private static T Convert(T value)
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (!BitConverter.IsLittleEndian)
            return value;

        if (typeof(T) == typeof(byte) ||
            typeof(T) == typeof(sbyte))
            return value;

        if (typeof(T) == typeof(short))
            return (T)(object)BinaryPrimitives.ReverseEndianness((short)(object)value);
        if (typeof(T) == typeof(ushort))
            return (T)(object)BinaryPrimitives.ReverseEndianness((ushort)(object)value);
        if (typeof(T) == typeof(int))
            return (T)(object)BinaryPrimitives.ReverseEndianness((int)(object)value);
        if (typeof(T) == typeof(uint))
            return (T)(object)BinaryPrimitives.ReverseEndianness((uint)(object)value);
        if (typeof(T) == typeof(nint))
            return (T)(object)BinaryPrimitives.ReverseEndianness((nint)(object)value);
        if (typeof(T) == typeof(nuint))
            return (T)(object)BinaryPrimitives.ReverseEndianness((nuint)(object)value);
        if (typeof(T) == typeof(long))
            return (T)(object)BinaryPrimitives.ReverseEndianness((long)(object)value);
        if (typeof(T) == typeof(ulong))
            return (T)(object)BinaryPrimitives.ReverseEndianness((ulong)(object)value);
        if (typeof(T) == typeof(Int128))
            return (T)(object)BinaryPrimitives.ReverseEndianness((Int128)(object)value);
        if (typeof(T) == typeof(UInt128))
            return (T)(object)BinaryPrimitives.ReverseEndianness((UInt128)(object)value);
        if (typeof(T) == typeof(char))
            return (T)(object)(char)BinaryPrimitives.ReverseEndianness((char)(object)value);

        throw new NotSupportedException();
#pragma warning restore IDE0046 // Convert to conditional expression
    }
}