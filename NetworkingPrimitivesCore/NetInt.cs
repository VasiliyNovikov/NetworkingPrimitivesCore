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
    private static T Convert(T value) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
}