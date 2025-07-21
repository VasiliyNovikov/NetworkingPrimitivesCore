using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NetworkingPrimitivesCore.Formatting;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal ref struct SpanReader<TChar>(ReadOnlySpan<TChar> source)
    where TChar : unmanaged, IBinaryInteger<TChar>, IUnsignedNumber<TChar>
{
    private readonly ReadOnlySpan<TChar> _source = source;

    [SuppressMessage("Microsoft.Style", "IDE0032: Use auto property", Justification = "Performance")]
    private int _position = 0;

    public readonly int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public readonly bool IsEndOfSource
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position == _source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool TryPeekCore(out TChar value)
    {
        if (IsEndOfSource)
        {
            Unsafe.SkipInit(out value);
            return false;
        }
        value = _source[_position];
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TOtherChar Convert<TOtherChar>(TChar value) where TOtherChar : unmanaged, INumberBase<TOtherChar> => TOtherChar.CreateTruncating(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySkip()
    {
        if (IsEndOfSource)
            return false;
        ++_position;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryPeek(out char value)
    {
        Unsafe.SkipInit(out value);
        if (typeof(TChar) == typeof(char))
            return TryPeekCore(out Unsafe.As<char, TChar>(ref value));

        if (!TryPeekCore(out var ch))
            return false;

        value = Convert<char>(ch);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead(out char value)
    {
        if (!TryPeek(out value))
            return false;
        ++_position;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadOne(char expectedValue)
    {
        if (TryPeek(out var value) && value == expectedValue)
        {
            ++_position;
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadDecimalDigit(out byte value)
    {
        if (!TryPeek(out var ch) || ch is < '0' or > '9')
        {
            Unsafe.SkipInit(out value);
            return false;
        }
        value = (byte)(ch - '0');
        ++_position;
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

        switch (ch)
        {
            case >= '0' and <= '9':
                value = (byte)(ch - '0');
                break;
            case >= 'a' and <= 'f':
                value = (byte)(ch - 'a' + 10);
                break;
            case >= 'A' and <= 'F':
                value = (byte)(ch - 'A' + 10);
                break;
            default:
                Unsafe.SkipInit(out value);
                return false;
        }

        ++_position;
        return true;
    }
}