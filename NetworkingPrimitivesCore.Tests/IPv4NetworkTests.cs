using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPv4NetworkTests
{
    [TestMethod]
    public void IPv4Network_Size_Test() => Assert.AreEqual(12, Unsafe.SizeOf<IPv4Network>());

    private static IEnumerable<object[]> IPv4Network_Parse_Test_Data() =>
    [
       ["10.10.128.0/22", "10.10.128.0", 22, "255.255.252.0"],
       ["10.10.128.0/21", "10.10.128.0", 21, "255.255.248.0"],
       ["10.10.128.0/20", "10.10.128.0", 20, "255.255.240.0"],
       ["10.10.128.3", "10.10.128.3", 32, "255.255.255.255"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Parse_Test_Data))]
    public void IPv4Network_Parse_Test(string networkString, string addressString, int prefix, string mask)
    {
        var network = IPv4Network.Parse(networkString);
        Assert.AreEqual(IPv4Address.Parse(addressString), network.Address);
        Assert.AreEqual(prefix, network.Prefix);
        Assert.AreEqual(IPv4Address.Parse(mask), network.Mask);
    }

    private static IEnumerable<object[]> IPv4Network_Parse_Failure_Test_Data() =>
    [
        ["4654yyrehh"],
        ["10.345.128.0/21"],
        ["10.10.128.0/xx"],
        ["10.10.128.0/0"],
        ["10.10.128.0/33"],
        ["192.168.1.1/24"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Parse_Failure_Test_Data))]
    public void IPv4Network_Parse_Failure_Test(string networkString)
    {
        Assert.ThrowsExactly<FormatException>(() => IPv4Network.Parse(networkString));
    }

    private static IEnumerable<object[]> IPv4Network_Contains_Test_Data() =>
    [
        ["10.10.128.0/22", "10.10.128.26", true],
        ["10.10.128.0/22", "10.11.128.26", false]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Contains_Test_Data))]
    public void IPv4Network_Contains_Test(string networkString, string addressString, bool contains)
    {
        var network = IPv4Network.Parse(networkString);
        var address = IPv4Address.Parse(addressString);
        Assert.AreEqual(contains, network.Contains(address), $"{address} is {(contains ? "" : "not ")}in the {network}");
    }

    private static IEnumerable<object[]> IPv4Network_Indexer_Test_Data() =>
    [
        ["10.10.128.0/22", 0u, "10.10.128.0"],
        ["10.10.128.0/22", 3u, "10.10.128.3"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Indexer_Test_Data))]
    public void IPv4Network_Indexer_Test(string networkString, uint index, string addressString)
    {
        var network = IPv4Network.Parse(networkString);
        Assert.AreEqual(addressString, network[index].ToString());
    }

    private static IEnumerable<object[]> IPv4Network_Subnet_Test_Data() =>
    [
        ["198.18.0.0/15", 20, 1, "198.18.16.0/20"],
        ["198.18.0.0/15", 20, 3, "198.18.48.0/20"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Subnet_Test_Data))]
    public void IPv4Network_Subnet_Test(string networkString, int prefix, int index, string subnetString)
    {
        var network = IPv4Network.Parse(networkString);
        var subnet = network.Subnet(prefix, index);
        Assert.AreEqual(subnetString, subnet.ToString());
    }

    private static IEnumerable<object[]> IPv4Network_Supernet_Test_Data() =>
    [
        ["198.18.16.0/20", 15, "198.18.0.0/15"],
        ["198.18.48.0/20", 15, "198.18.0.0/15"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_Supernet_Test_Data))]
    public void IPv4Network_Supernet_Test(string networkString, int prefix, string supernetString)
    {
        var network = IPv4Network.Parse(networkString);
        var supernet = network.Supernet(prefix);
        Assert.AreEqual(supernetString, supernet.ToString());
    }

    private static IEnumerable<object[]> IPv4Network_ToString_Test_Data() =>
    [
        ["10.10.128.0/22"],
        ["1.0.0.0/8"],
        ["1.2.3.0/24"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv4Network_ToString_Test_Data))]
    public void IPv4Network_ToString_Test(string networkString)
    {
        var network = IPv4Network.Parse(networkString);
        Assert.AreEqual(networkString, network.ToString());
    }
}