using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetworkingPrimitivesCore;

// A lot of things copied from System.UInt64
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct UInt48
    : IBinaryInteger<UInt48>
    , IUnsignedNumber<UInt48>
    , IMinMaxValue<UInt48>
    , IEquatable<UInt48>
    , IComparable<UInt48>
{
    [InlineArray(3)]
    private struct Data { private ushort _value0; }

    private const ulong UlongMaxValue = 0xFFFFFFFFFFFF;

    private readonly Data _data;

    private ulong Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ulong result = 0;
            Unsafe.As<ulong, Data>(ref result) = _data;
            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private UInt48(ulong value) => _data = Unsafe.As<ulong, Data>(ref Unsafe.AsRef(in value));

    public static UInt48 One { get; } = new UInt48(1);

    public static int Radix { get; } = 2;

    public static UInt48 Zero { get; } = new(0);

    public static UInt48 AdditiveIdentity { get; } = Zero;

    public static UInt48 MultiplicativeIdentity { get; } = One;

    public static UInt48 MaxValue { get; } = new(UlongMaxValue);

    public static UInt48 MinValue { get; } = Zero;

    public static UInt48 Abs(UInt48 value) => value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCanonical(UInt48 value) => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsComplexNumber(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEvenInteger(UInt48 value) => (value.Value & 1) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(UInt48 value) => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsImaginaryNumber(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinity(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInteger(UInt48 value) => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegativeInfinity(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNormal(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOddInteger(UInt48 value) => (value.Value & 1) == 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(UInt48 value) => value.Value > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositiveInfinity(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(UInt48 value) => ulong.IsPow2(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRealNumber(UInt48 value) => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubnormal(UInt48 value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(UInt48 value) => value.Value == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 Log2(UInt48 value) => new((ulong)BitOperations.Log2(value.Value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 MaxMagnitude(UInt48 x, UInt48 y) => x > y ? x : y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 MaxMagnitudeNumber(UInt48 x, UInt48 y) => x > y ? x : y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 MinMagnitude(UInt48 x, UInt48 y) => x < y ? x : y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 MinMagnitudeNumber(UInt48 x, UInt48 y) => x < y ? x : y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        var value = ulong.Parse(s, style, provider);
        return value > UlongMaxValue
            ? throw new ArgumentOutOfRangeException(nameof(s), "Value must be between 0 and 2^48 - 1.")
            : new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse(s.AsSpan(), style, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 PopCount(UInt48 value) => new(ulong.PopCount(value.Value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 TrailingZeroCount(UInt48 value) => new(ulong.TrailingZeroCount(value.Value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out UInt48 result) where TOther : INumberBase<TOther>
    {
        var success = TOther.TryConvertToChecked(value, out ulong ulongResult) && ulongResult <= UlongMaxValue;
        result = success ? new(ulongResult) : default;
        return success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out UInt48 result) where TOther : INumberBase<TOther>
    {
        var success = TOther.TryConvertToSaturating(value, out ulong ulongResult);
        result = success ? new(ulongResult <= UlongMaxValue ? ulongResult : UlongMaxValue) : default;
        return success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out UInt48 result) where TOther : INumberBase<TOther>
    {
        var success = TOther.TryConvertToTruncating(value, out ulong ulongResult);
        result = success ? new(ulongResult) : default;
        return success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertToChecked<TOther>(UInt48 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromChecked(value.Value, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertToSaturating<TOther>(UInt48 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromSaturating(value.Value, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvertToTruncating<TOther>(UInt48 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromTruncating(value.Value, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out UInt48 result)
    {
        if (ulong.TryParse(s, style, provider, out ulong value) && value <= UlongMaxValue)
        {
            result = new UInt48(value);
            return true;
        }
        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out UInt48 result) => TryParse(s.AsSpan(), style, provider, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out UInt48 result) => TryParse(s, NumberStyles.Integer, provider, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out UInt48 result) => TryParse(s.AsSpan(), provider, out result);


    // Copied from System.UInt64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt48 value)
    {
        if (source.Length == 0)
        {
            value = default;
            return true;
        }

        if (!isUnsigned && sbyte.IsNegative((sbyte)source[0]))
        {
            value = default;
            return false;
        }

        if ((source.Length > Unsafe.SizeOf<UInt48>()) && source[..^Unsafe.SizeOf<UInt48>()].ContainsAnyExcept((byte)0x00))
        {
            value = default;
            return false;
        }

        ref byte sourceRef = ref MemoryMarshal.GetReference(source);

        if (source.Length >= Unsafe.SizeOf<UInt48>())
        {
            sourceRef = ref Unsafe.Add(ref sourceRef, source.Length - Unsafe.SizeOf<UInt48>());
            var result = Unsafe.ReadUnaligned<UInt48>(ref sourceRef);
            if (BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            value = result;
        }
        else
        {
            ulong ulongResult = 0;
            for (int i = 0; i < source.Length; i++)
            {
                ulongResult <<= 8;
                ulongResult |= Unsafe.Add(ref sourceRef, i);
            }
            value = new(ulongResult);
        }
        return true;
    }

    // Copied from System.UInt64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt48 value)
    {
        if (source.Length == 0)
        {
            value = default;
            return true;
        }

        if (!isUnsigned && sbyte.IsNegative((sbyte)source[^1]))
        {
            value = default;
            return false;
        }

        if ((source.Length > Unsafe.SizeOf<UInt48>()) && source[Unsafe.SizeOf<UInt48>()..].ContainsAnyExcept((byte)0x00))
        {
            value = default;
            return false;
        }

        ref byte sourceRef = ref MemoryMarshal.GetReference(source);

        if (source.Length >= Unsafe.SizeOf<UInt48>())
        {
            var result = Unsafe.ReadUnaligned<UInt48>(ref sourceRef);
            if (!BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            value = result;
        }
        else
        {
            ulong ulongResult = 0;
            for (int i = 0; i < source.Length; i++)
            {
                ulong part = Unsafe.Add(ref sourceRef, i);
                part <<= i * 8;
                ulongResult |= part;
            }
            value = new(ulongResult);
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj) => obj is UInt48 other ? CompareTo(other) : throw new ArgumentException("Object must be of type UInt48.", nameof(obj));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(UInt48 other) => Value < other.Value ? -1 : Value > other.Value ? 1 : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(UInt48 other) => Value == other.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is UInt48 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetByteCount() => Unsafe.SizeOf<UInt48>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetShortestBitLength() => (sizeof(ulong) * 8) - BitOperations.LeadingZeroCount(Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length >= Unsafe.SizeOf<UInt48>())
        {
            var value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(this) : this;
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            bytesWritten = Unsafe.SizeOf<UInt48>();
            return true;
        }
        else
        {
            bytesWritten = 0;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length >= Unsafe.SizeOf<UInt48>())
        {
            var value = BitConverter.IsLittleEndian ? this : BinaryPrimitives.ReverseEndianness(this);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            bytesWritten = Unsafe.SizeOf<UInt48>();
            return true;
        }
        else
        {
            bytesWritten = 0;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator +(UInt48 value) => value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator +(UInt48 left, UInt48 right) => new(left.Value + right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator -(UInt48 value) => new(UlongMaxValue - value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator -(UInt48 left, UInt48 right) => new(left.Value - right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator ~(UInt48 value) => new(~value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator ++(UInt48 value) => new(value.Value + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator --(UInt48 value) => new(value.Value - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator *(UInt48 left, UInt48 right) => new(left.Value * right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator /(UInt48 left, UInt48 right) => new(left.Value / right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator %(UInt48 left, UInt48 right) => new(left.Value % right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator &(UInt48 left, UInt48 right) => new(left.Value & right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator |(UInt48 left, UInt48 right) => new(left.Value | right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator ^(UInt48 left, UInt48 right) => new(left.Value ^ right.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator <<(UInt48 value, int shiftAmount) => new(value.Value << shiftAmount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator >>(UInt48 value, int shiftAmount) => new(value.Value >> shiftAmount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(UInt48 left, UInt48 right) => left.Value == right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(UInt48 left, UInt48 right) => left.Value != right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(UInt48 left, UInt48 right) => left.Value < right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(UInt48 left, UInt48 right) => left.Value > right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(UInt48 left, UInt48 right) => left.Value <= right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(UInt48 left, UInt48 right) => left.Value >= right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt48 operator >>>(UInt48 value, int shiftAmount) => new(value.Value >>> shiftAmount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(long value) => new((ulong)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator long(UInt48 value) => (long)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(ulong value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ulong(UInt48 value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(int value) => new((ulong)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator int(UInt48 value) => (int)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt48(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(UInt48 value) => (uint)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(short value) => new((ulong)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator short(UInt48 value) => (short)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt48(ushort value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ushort(UInt48 value) => (ushort)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt48(byte value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator byte(UInt48 value) => (byte)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(sbyte value) => new((ulong)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator sbyte(UInt48 value) => (sbyte)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(nint value) => new((ulong)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nint(UInt48 value) => (nint)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt48(nuint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nuint(UInt48 value) => (nuint)value.Value;
}