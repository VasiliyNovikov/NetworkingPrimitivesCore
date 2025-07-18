using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

internal static class NetIntConvertibleOperatorHelper<T, TInt>
    where T : unmanaged, INetIntConvertible<T, TInt>
    where TInt : unmanaged, IBinaryInteger<TInt>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(T value) => ((NetInt<TInt>)value).GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal(T a, T b) => (NetInt<TInt>)a == (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual(T a, T b) => (NetInt<TInt>)a != (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(T a, T b) => ((NetInt<TInt>)a).CompareTo((NetInt<TInt>)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan(T a, T b) => (NetInt<TInt>)a < (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan(T a, T b) => (NetInt<TInt>)a > (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual(T a, T b) => (NetInt<TInt>)a <= (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual(T a, T b) => (NetInt<TInt>)a >= (NetInt<TInt>)b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Not(T value) => (T)~(NetInt<TInt>)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T And(T a, T b) => (T)((NetInt<TInt>)a & (NetInt<TInt>)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Or(T a, T b) => (T)((NetInt<TInt>)a | (NetInt<TInt>)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Xor(T a, T b) => (T)((NetInt<TInt>)a ^ (NetInt<TInt>)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInt ToInt(T value) => (TInt)(NetInt<TInt>)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FromInt(TInt value) => (T)(NetInt<TInt>)value;
}