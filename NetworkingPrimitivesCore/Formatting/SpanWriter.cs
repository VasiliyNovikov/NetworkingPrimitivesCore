using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal ref struct SpanWriter<TChar>(Span<TChar> destination)
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    [InlineArray(16)]
    private struct CharArray16 { private TChar _e0; }

    private static readonly CharArray16 LowerHexLookup;
    private static readonly CharArray16 UpperHexLookup;

    static SpanWriter()
    {
        for (byte i = 0; i < 10; ++i)
            LowerHexLookup[i] = UpperHexLookup[i] = TChar.CreateTruncating('0' + i);
        for (byte i = 0; i < 6; ++i)
        {
            LowerHexLookup[10 + i] = TChar.CreateTruncating('a' + i);
            UpperHexLookup[10 + i] = TChar.CreateTruncating('A' + i);
        }
    }

    private readonly Span<TChar> _destination = destination;

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [SuppressMessage("Style", "IDE0251: Make member readonly", Justification = "False positive")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(TChar value)
    {
        if (Position == _destination.Length)
            return false;

        _destination[Position++] = value;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(ReadOnlySpan<TChar> value)
    {
        if (!value.TryCopyTo(_destination[Position..]))
            return false;

        Position += value.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteDecimalDigit(byte digit) => TryWrite(TChar.CreateTruncating('0' + digit));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteHexDigit(byte digit, bool isUpper = false) => TryWrite(isUpper ? UpperHexLookup[digit] : LowerHexLookup[digit]);
}