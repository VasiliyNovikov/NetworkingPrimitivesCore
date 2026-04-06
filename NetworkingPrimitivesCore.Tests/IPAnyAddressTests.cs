using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPAnyAddressTests
{
    [TestMethod]
    public void IPAnyAddress_Size_Test() => Assert.AreEqual(20, Unsafe.SizeOf<IPAnyAddress>());

    private static object[][] Test_IPAddresses() =>
    [
        ["127.0.0.1"],
        ["192.168.0.1"],
        ["::"],
        ["::1"],
        ["fec0::1"],
        ["2001:db8::1"],
        ["2001:db8:0:1:1:1:1:1"],
        ["2001:db8:85a3:0:1:8a2e:370:7334"],
        ["2001:db8::1:0:0:1"],
        ["::ffff:192.168.0.1"]
    ];

    [TestMethod]
    [DynamicData(nameof(Test_IPAddresses))]
    public void IPAnyAddress_Parse_Format_Test(string address)
    {
        var fwAddress = IPAddress.Parse(address);
        var ipAddress = IPAnyAddress.Parse(address);

        var expectedAddressStr = fwAddress.ToString();
        var actualAddressStr = ipAddress.ToString();

        CollectionAssert.AreEqual(fwAddress.GetAddressBytes(), ipAddress.Bytes.ToArray());
        Assert.AreEqual(expectedAddressStr, actualAddressStr);
    }

    [TestMethod]
    [DataRow("0.0.0.0", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
    [DataRow("::", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 })]
    public void IPAnyAddress_MemoryLayout_Test(string addressStr, byte[] expectedLayout)
    {
        var address = IPAnyAddress.Parse(addressStr);
        var layout = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref address, 1)).ToArray();
        CollectionAssert.AreEqual(expectedLayout, layout);
    }
}