using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

internal static class OperatorHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T a, T b) where T : IEqualityOperators<T, T, bool> => a == b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<T>(T a, T b) where T : IEqualityOperators<T, T, bool> => a != b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a < b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a > b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a <= b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a >= b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Not<T>(T value) where T : IBitwiseOperators<T, T, T> => ~value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T And<T>(T a, T b) where T : IBitwiseOperators<T, T, T> => a & b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Or<T>(T a, T b) where T : IBitwiseOperators<T, T, T> => a | b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Xor<T>(T a, T b) where T : IBitwiseOperators<T, T, T> => a ^ b;
}