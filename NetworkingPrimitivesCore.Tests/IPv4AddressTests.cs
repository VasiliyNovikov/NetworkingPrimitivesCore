using System.Net;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPv4AddressTests
{
    [TestMethod]
    public void IPv4Address_Size_Test() => Assert.AreEqual(4, Unsafe.SizeOf<IPv4Address>());
    
    [TestMethod]
    public void IPv4Address_Broadcast_Test() => Assert.AreEqual(IPv4Address.Parse("255.255.255.255"), IPv4Address.Broadcast);

    private static object[][] Test_IPAddresses() =>
    [
        ["127.0.0.1"],
        ["192.168.0.1"]
    ];

    [TestMethod]
    [DynamicData(nameof(Test_IPAddresses))]
    public void IPv4Address_Parse_Format_Test(string address)
    {
        var fwAddress = IPAddress.Parse(address);
        var ipAddress = IPv4Address.Parse(address);

        var expectedAddressStr = fwAddress.ToString();
        var actualAddressStr = ipAddress.ToString();

        CollectionAssert.AreEqual(fwAddress.GetAddressBytes(), ipAddress.Bytes.ToArray());
        Assert.AreEqual(expectedAddressStr, actualAddressStr);
    }
}