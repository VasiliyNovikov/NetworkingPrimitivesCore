#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Numerics;
using System.Runtime.CompilerServices;

using NetworkingPrimitivesCore;

namespace System.Buffers.Binary;

public static class BinaryPrimitivesExtensions
{
    extension(BinaryPrimitives)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt48 ReverseEndianness(UInt48 value)
        {
            ulong ulongValue = (ulong)value;
            ulong reversedValue = BinaryPrimitives.ReverseEndianness(ulongValue);
            return (UInt48)(reversedValue >> 16);
        }

        // It was benchmarked in ReverseEndiannessBenchmarks and showed
        // to have the same performance as direct call of BinaryPrimitives.ReverseEndianness for specific types
        // and to not do any memory allocations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReverseEndianness<T>(T value) where T : unmanaged, IBinaryInteger<T>
        {
#pragma warning disable IDE0046 // Convert to conditional expression
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
            if (typeof(T) == typeof(UInt48))
                return (T)(object)BinaryPrimitives.ReverseEndianness((UInt48)(object)value);
            if (typeof(T) == typeof(char))
                return (T)(object)(char)BinaryPrimitives.ReverseEndianness((char)(object)value);

            throw new NotSupportedException();
#pragma warning restore IDE0046 // Convert to conditional expression
        }
    }
}