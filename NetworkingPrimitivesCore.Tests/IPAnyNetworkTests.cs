using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPAnyNetworkTests
{
    [TestMethod]
    public void IPNetwork_Size_Test() => Assert.AreEqual(40, Unsafe.SizeOf<IPAnyNetwork>());

    private static IEnumerable<object[]> IPNetwork_Parse_Test_Data() =>
    [
       ["10.10.128.0/22", "10.10.128.0", 22, "255.255.252.0"],
       ["10.10.128.0/21", "10.10.128.0", 21, "255.255.248.0"],
       ["10.10.128.0/20", "10.10.128.0", 20, "255.255.240.0"],
       ["10.10.128.3", "10.10.128.3", 32, "255.255.255.255"],
       ["fec0::/64", "fec0::", 64, "ffff:ffff:ffff:ffff::" ],
       ["2a01:110:1008:b:45b1:911f:4b9a:48e7", "2a01:110:1008:b:45b1:911f:4b9a:48e7", 128, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"],
       ["2001:db8::/32", "2001:db8::", 32, "ffff:ffff:0:0:0:0:0:0"],
       ["2001:db8::1/128", "2001:db8::1", 128, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Parse_Test_Data))]
    public void IPNetwork_Parse_Test(string networkString, string addressString, int prefix, string mask)
    {
        var network = IPAnyNetwork.Parse(networkString);
        Assert.AreEqual(IPAnyAddress.Parse(addressString), network.Address);
        Assert.AreEqual(prefix, network.Prefix);
        Assert.AreEqual(IPAnyAddress.Parse(mask), network.Mask);
    }

    private static IEnumerable<object[]> IPNetwork_Parse_Failure_Test_Data() =>
    [
        ["4654yyrehh"],
        ["10.345.128.0/21"],
        ["10.10.128.0/xx"],
        ["10.10.128.0/0"],
        ["10.10.128.0/33"],
        ["192.168.1.1/24"],
        ["2001:db8::/129"],
        ["2001:db8::/xx"],
        ["2001:db8::/0"],
        ["2001:db8::/8"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Parse_Failure_Test_Data))]
    public void IPNetwork_Parse_Failure_Test(string networkString)
    {
        Assert.ThrowsExactly<FormatException>(() => IPAnyNetwork.Parse(networkString));
    }

    private static IEnumerable<object[]> IPNetwork_Contains_Test_Data() =>
    [
        ["10.10.128.0/22", "10.10.128.26", true],
        ["10.10.128.0/22", "10.11.128.26", false],
        ["fec0::/64", "fec0::da94", true],
        ["fec0::/64", "fec1::da94", false]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Contains_Test_Data))]
    public void IPNetwork_Contains_Test(string networkString, string addressString, bool contains)
    {
        var network = IPAnyNetwork.Parse(networkString);
        var address = IPAnyAddress.Parse(addressString);
        Assert.AreEqual(contains, network.Contains(address), $"{address} is {(contains ? "" : "not ")}in the {network}");
    }

    private static IEnumerable<object[]> IPNetwork_Indexer_Test_Data() =>
    [
        ["10.10.128.0/22", 0u, "10.10.128.0"],
        ["10.10.128.0/22", 3u, "10.10.128.3"],
        ["fec0::/64", 0u, "fec0::"],
        ["fec0::/64", 123456789u, "fec0::75b:cd15"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Indexer_Test_Data))]
    public void IPNetwork_Indexer_Test(string networkString, uint index, string addressString)
    {
        var network = IPAnyNetwork.Parse(networkString);
        Assert.AreEqual(addressString, network[index].ToString());
    }

    private static IEnumerable<object[]> IPNetwork_Subnet_Test_Data() =>
    [
        ["198.18.0.0/15", 20, 1, "198.18.16.0/20"],
        ["198.18.0.0/15", 20, 3, "198.18.48.0/20"],
        ["fec1::/64", 69, 1, "fec1::800:0:0:0/69"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Subnet_Test_Data))]
    public void IPNetwork_Subnet_Test(string networkString, int prefix, int index, string subnetString)
    {
        var network = IPAnyNetwork.Parse(networkString);
        var subnet = network.Subnet(prefix, index);
        Assert.AreEqual(subnetString, subnet.ToString());
    }

    private static IEnumerable<object[]> IPNetwork_Supernet_Test_Data() =>
    [
        ["198.18.16.0/20", 15, "198.18.0.0/15"],
        ["198.18.48.0/20", 15, "198.18.0.0/15"],
        ["fec1::800:0:0:0/69", 64, "fec1::/64"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_Supernet_Test_Data))]
    public void IPNetwork_Supernet_Test(string networkString, int prefix, string supernetString)
    {
        var network = IPAnyNetwork.Parse(networkString);
        var supernet = network.Supernet(prefix);
        Assert.AreEqual(supernetString, supernet.ToString());
    }

    private static IEnumerable<object[]> IPNetwork_ToString_Test_Data() =>
    [
        ["10.10.128.0/22"],
        ["1.0.0.0/8"],
        ["1.2.3.0/24"],
        ["fec0::/64"],
        ["2001:db8::/32"],
        ["2001:db8::1/128"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPNetwork_ToString_Test_Data))]
    public void IPNetwork_ToString_Test(string networkString)
    {
        var network = IPAnyNetwork.Parse(networkString);
        Assert.AreEqual(networkString, network.ToString());
    }
}