using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore.Formatting;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct SpanWriter<TChar>(Span<TChar> destination)
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    private readonly Span<TChar> _destination = destination;

    [SuppressMessage("Microsoft.Style", "IDE0032: Use auto property", Justification = "Performance")]
    private int _position = 0;

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryWriteCore(TChar value)
    {
        if (_position == _destination.Length)
            return false;

        _destination[_position++] = value;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(char value)
    {
        return typeof(TChar) == typeof(char)
            ? TryWriteCore((TChar)(object)value)
            : TryWriteCore(TChar.CreateTruncating(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(ReadOnlySpan<char> value)
    {
        if (value.Length > _destination.Length - _position)
            return false;

        if (typeof(TChar) == typeof(char))
            MemoryMarshal.Cast<char, TChar>(value).CopyTo(_destination[_position..]);
        else
            foreach (var ch in value)
                TryWrite(ch);
        _position += value.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteDecimalDigit(byte digit) => TryWrite((char)('0' + digit));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteHexDigit(byte digit, bool isUpper = false) => TryWrite((char)(digit < 10 ? '0' + digit : (isUpper ? 'A' : 'a') + digit - 10));
}
