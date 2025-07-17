using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal ref struct SpanWriter<TChar>(Span<TChar> destination)
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
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
        if (value.Length > _destination.Length - Position)
            return false;
        value.CopyTo(_destination[Position..]);
        Position += value.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteDecimalDigit(byte digit) => TryWrite(TChar.CreateTruncating('0' + digit));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteHexDigit(byte digit, bool isUpper = false) => TryWrite(TChar.CreateTruncating(digit < 10 ? '0' + digit : (isUpper ? 'A' : 'a') - 10 + digit));
}