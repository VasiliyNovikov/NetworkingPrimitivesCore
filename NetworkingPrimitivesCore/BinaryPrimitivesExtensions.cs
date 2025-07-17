#pragma warning disable IDE0130 // Namespace does not match folder structure
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
    }
}