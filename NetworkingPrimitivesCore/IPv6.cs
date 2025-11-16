using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public sealed class IPv6 : IIPVersion<IPv6>
{
    public static byte Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 6;
    }
}