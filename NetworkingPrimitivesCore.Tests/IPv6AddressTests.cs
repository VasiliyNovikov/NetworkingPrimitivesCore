using System.Net;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPv6AddressTests
{
    [TestMethod]
    public void IPv6Address_Size_Test() => Assert.AreEqual(16, Unsafe.SizeOf<IPv6Address>());

    [TestMethod]
    public void IPv6Address_Broadcast_Test() => Assert.AreEqual(IPv6Address.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"), IPv6Address.Broadcast);

    private static object[][] Test_IPAddresses() =>
    [
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
    public void IPv6Address_Parse_Format_Test(string address)
    {
        var fwAddress = IPAddress.Parse(address);
        var ipAddress = IPv6Address.Parse(address);

        var expectedAddressStr = fwAddress.ToString();
        var actualAddressStr = ipAddress.ToString();

        CollectionAssert.AreEqual(fwAddress.GetAddressBytes(), ipAddress.Bytes.ToArray());
        Assert.AreEqual(expectedAddressStr, actualAddressStr);
    }
}