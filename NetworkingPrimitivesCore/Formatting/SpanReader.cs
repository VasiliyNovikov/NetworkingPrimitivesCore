using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal ref struct SpanReader<TChar>(ReadOnlySpan<TChar> source)
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    [InlineArray(256)]
    private struct ByteArray256 { private byte _e0; }

    private static readonly ByteArray256 HexLookup;

    static SpanReader()
    {
        ((Span<byte>)HexLookup).Fill(byte.MaxValue);
        for (byte i = 0; i < 10; ++i)
            HexLookup['0' + i] = i;
        for (byte i = 0; i < 6; ++i)
            HexLookup['A' + i] = HexLookup['a' + i] = (byte)(10 + i);
    }

    private readonly ReadOnlySpan<TChar> _source = source;

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [SuppressMessage("Style", "IDE0251: Make member readonly", Justification = "False positive")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public readonly bool IsEndOfSource
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Position == _source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool TryPeek(out TChar value)
    {
        if (IsEndOfSource)
        {
            Unsafe.SkipInit(out value);
            return false;
        }
        value = _source[Position];
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead(out TChar value)
    {
        if (!TryPeek(out value))
            return false;
        ++Position;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadOne(TChar expectedValue)
    {
        if (TryPeek(out var value) && value == expectedValue)
        {
            ++Position;
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadDecimalDigit(out byte value)
    {
        if (!TryPeek(out var ch))
        {
            Unsafe.SkipInit(out value);
            return false;
        }

        var intValue = uint.CreateTruncating(ch) - '0';
        if (intValue > 9)
        {
            Unsafe.SkipInit(out value);
            return false;
        }
        value = (byte)intValue;
        ++Position;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadHexDigit(out byte value)
    {
        if (!TryPeek(out var ch))
        {
            Unsafe.SkipInit(out value);
            return false;
        }

        var intCh = uint.CreateTruncating(ch);
        if (intCh > 0x7F)
        {
            Unsafe.SkipInit(out value);
            return false;
        }

        value = HexLookup[(int)intCh];
        if (value == byte.MaxValue)
        {
            Unsafe.SkipInit(out value);
            return false;
        }

        ++Position;
        return true;
    }
}