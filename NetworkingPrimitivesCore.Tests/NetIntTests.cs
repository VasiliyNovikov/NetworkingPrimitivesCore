using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

// Tests are AI generated and may not cover all edge cases.
[TestClass]
public class NetIntTests
{
    [TestMethod]
    public void NetInt_Constructor_ShouldConvertEndianness()
    {
        // Test with 16-bit values - round trip should preserve original value
        var originalShort = (short)0x1234;
        var netShort = (NetInt<short>)originalShort;
        var backShort = (short)netShort;
        Assert.AreEqual(originalShort, backShort);

        // Test with 32-bit values - round trip should preserve original value
        var originalInt = 0x12345678;
        var netInt = (NetInt<int>)originalInt;
        var backInt = (int)netInt;
        Assert.AreEqual(originalInt, backInt);
    }

    [TestMethod]
    public void NetInt_ByteTypes_ShouldNotChangeEndianness()
    {
        // Bytes should not be affected by endianness conversion
        var netByte = (NetInt<byte>)0x12;
        Assert.AreEqual((byte)0x12, (byte)netByte);

        var netSByte = (NetInt<sbyte>)0x12;
        Assert.AreEqual((sbyte)0x12, (sbyte)netSByte);
    }

    [TestMethod]
    public void NetInt_Equality_ShouldWork()
    {
        var netInt1 = (NetInt<int>)0x12345678;
        var netInt2 = (NetInt<int>)0x12345678;
        var netInt3 = (NetInt<int>)unchecked((int)0x87654321);

        Assert.AreEqual(netInt1, netInt2);
        Assert.AreNotEqual(netInt1, netInt3);
        Assert.IsTrue(netInt1 == netInt2);
        Assert.IsTrue(netInt1 != netInt3);
        Assert.IsFalse(netInt1 == netInt3);
        Assert.IsFalse(netInt1 != netInt2);
    }

    [TestMethod]
    public void NetInt_Comparison_ShouldWork()
    {
        var smaller = (NetInt<int>)100;
        var larger = (NetInt<int>)200;

        Assert.IsTrue(smaller < larger);
        Assert.IsTrue(smaller <= larger);
        Assert.IsTrue(larger > smaller);
        Assert.IsTrue(larger >= smaller);
        Assert.IsFalse(smaller > larger);
        Assert.IsFalse(larger < smaller);

        Assert.AreEqual(-1, smaller.CompareTo(larger));
        Assert.AreEqual(1, larger.CompareTo(smaller));
        Assert.AreEqual(0, smaller.CompareTo(smaller));
    }

    [TestMethod]
    public void NetInt_BitwiseOperations_ShouldWork()
    {
        var value1 = (NetInt<int>)0b1010;
        var value2 = (NetInt<int>)0b1100;

        var andResult = value1 & value2;
        var orResult = value1 | value2;
        var xorResult = value1 ^ value2;
        var notResult = ~value1;

        // The bitwise operations work on the raw values (before endianness conversion)
        Assert.AreEqual((NetInt<int>)(0b1010 & 0b1100), andResult);
        Assert.AreEqual((NetInt<int>)(0b1010 | 0b1100), orResult);
        Assert.AreEqual((NetInt<int>)(0b1010 ^ 0b1100), xorResult);
        Assert.AreEqual((NetInt<int>)~0b1010, notResult);
    }

    [TestMethod]
    public void NetInt_HashCode_ShouldBeConsistent()
    {
        var netInt1 = (NetInt<int>)0x12345678;
        var netInt2 = (NetInt<int>)0x12345678;
        var netInt3 = (NetInt<int>)unchecked((int)0x87654321);

        Assert.AreEqual(netInt1.GetHashCode(), netInt2.GetHashCode());
        Assert.AreNotEqual(netInt1.GetHashCode(), netInt3.GetHashCode());
    }

    [TestMethod]
    public void NetInt_EqualsObject_ShouldWork()
    {
        var netInt1 = (NetInt<int>)0x12345678;
        var netInt2 = (NetInt<int>)0x12345678;
        var netInt3 = (NetInt<int>)unchecked((int)0x87654321);

        Assert.IsTrue(netInt1.Equals((object)netInt2));
        Assert.IsFalse(netInt1.Equals((object)netInt3));
        Assert.IsFalse(netInt1.Equals(null));
        Assert.IsFalse(netInt1.Equals("string"));
    }

    [TestMethod]
    public void NetInt_UnsignedTypes_ShouldWork()
    {
        var originalUShort = (ushort)0x1234;
        var originalUInt = 0x12345678u;
        var originalULong = 0x123456789ABCDEFul;

        var netUShort = (NetInt<ushort>)originalUShort;
        var netUInt = (NetInt<uint>)originalUInt;
        var netULong = (NetInt<ulong>)originalULong;

        // Verify round-trip conversion preserves values
        Assert.AreEqual(originalUShort, (ushort)netUShort);
        Assert.AreEqual(originalUInt, (uint)netUInt);
        Assert.AreEqual(originalULong, (ulong)netULong);
    }

    [TestMethod]
    public void NetInt_LongTypes_ShouldWork()
    {
        var originalLong = 0x123456789ABCDEF0L;
        var netLong = (NetInt<long>)originalLong;
        var backLong = (long)netLong;

        // Verify round-trip conversion preserves value
        Assert.AreEqual(originalLong, backLong);
    }

    [TestMethod]
    public void NetInt_CharType_ShouldWork()
    {
        var originalChar = 'A';
        var netChar = (NetInt<char>)originalChar;
        var backChar = (char)netChar;

        // Verify round-trip conversion preserves value
        Assert.AreEqual(originalChar, backChar);
    }

    [TestMethod]
    public void NetInt_Int128Types_ShouldWork()
    {
        var originalValue = new Int128(0x123456789ABCDEF0, 0x0FEDCBA987654321);
        var netInt128 = (NetInt<Int128>)originalValue;
        var backInt128 = (Int128)netInt128;

        // Verify round-trip conversion preserves value
        Assert.AreEqual(originalValue, backInt128);
    }

    [TestMethod]
    public void NetInt_UInt128Types_ShouldWork()
    {
        var originalValue = new UInt128(0x123456789ABCDEF0, 0x0FEDCBA987654321);
        var netUInt128 = (NetInt<UInt128>)originalValue;
        var backUInt128 = (UInt128)netUInt128;

        // Verify round-trip conversion preserves value
        Assert.AreEqual(originalValue, backUInt128);
    }

    [TestMethod]
    public void NetInt_NativeIntTypes_ShouldWork()
    {
        var originalNInt = (nint)0x12345678;
        var originalNUInt = (nuint)0x12345678u;

        var netNInt = (NetInt<nint>)originalNInt;
        var netNUInt = (NetInt<nuint>)originalNUInt;

        var backNInt = (nint)netNInt;
        var backNUInt = (nuint)netNUInt;

        // Verify round-trip conversion preserves values
        Assert.AreEqual(originalNInt, backNInt);
        Assert.AreEqual(originalNUInt, backNUInt);
    }

    [TestMethod]
    public void NetInt_ZeroValues_ShouldWork()
    {
        var netIntZero = (NetInt<int>)0;
        var netLongZero = (NetInt<long>)0L;
        var netShortZero = (NetInt<short>)0;

        Assert.AreEqual(0, (int)netIntZero);
        Assert.AreEqual(0L, (long)netLongZero);
        Assert.AreEqual((short)0, (short)netShortZero);
    }

    [TestMethod]
    public void NetInt_NetworkRepresentation_ShouldBeDifferentOnLittleEndian()
    {
        // This test verifies that the internal representation is actually in network byte order
        var originalValue = 0x12345678;
        var netInt = (NetInt<int>)originalValue;

        // Use Unsafe.BitCast to access the internal _value field directly
        var internalValue = Unsafe.BitCast<NetInt<int>, int>(netInt);

        if (BitConverter.IsLittleEndian)
        {
            // On little-endian systems, the internal representation should be different
            // because it's stored in big-endian format
            Assert.AreNotEqual(originalValue, internalValue);

            // Verify it's actually the byte-swapped version
            var expectedInternalValue = BinaryPrimitives.ReverseEndianness(originalValue);
            Assert.AreEqual(expectedInternalValue, internalValue);
        }
        else
        {
            // On big-endian systems, no conversion occurs
            Assert.AreEqual(originalValue, internalValue);
        }

        // But the extracted value should always match the original
        Assert.AreEqual(originalValue, (int)netInt);
    }

    [TestMethod]
    public void NetInt_MaxValues_ShouldWork()
    {
        var netIntMax = (NetInt<int>)int.MaxValue;
        var netLongMax = (NetInt<long>)long.MaxValue;
        var netShortMax = (NetInt<short>)short.MaxValue;

        // Verify round-trip conversion preserves values
        Assert.AreEqual(int.MaxValue, (int)netIntMax);
        Assert.AreEqual(long.MaxValue, (long)netLongMax);
        Assert.AreEqual(short.MaxValue, (short)netShortMax);
    }
}