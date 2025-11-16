using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public sealed class IPv4 : IIPVersion<IPv4>
{
    public static byte Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }
}