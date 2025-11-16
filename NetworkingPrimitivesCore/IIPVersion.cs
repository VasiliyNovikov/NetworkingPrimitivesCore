using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore;

public interface IIPVersion<V>
    where V : IIPVersion<V>
{
    static abstract byte Version { get; }
}

public interface IIPVersioned<V> where V : IIPVersion<V>
{
    static virtual byte Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => V.Version;
    }
}