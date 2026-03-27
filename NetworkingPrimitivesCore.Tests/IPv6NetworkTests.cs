using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class IPv6NetworkTests
{
    [TestMethod]
    public void IPv6Network_Size_Test() => Assert.AreEqual(36, Unsafe.SizeOf<IPv6Network>());

    private static IEnumerable<object[]> IPv6Network_Parse_Test_Data() =>
    [
       ["fec0::/64", "fec0::", 64, "ffff:ffff:ffff:ffff::" ],
       ["2a01:110:1008:b:45b1:911f:4b9a:48e7", "2a01:110:1008:b:45b1:911f:4b9a:48e7", 128, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"],
       ["2001:db8::/32", "2001:db8::", 32, "ffff:ffff:0:0:0:0:0:0"],
       ["2001:db8::1/128", "2001:db8::1", 128, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Parse_Test_Data))]
    public void IPv6Network_Parse_Test(string networkString, string addressString, int prefix, string mask)
    {
        var network = IPv6Network.Parse(networkString);
        Assert.AreEqual(IPv6Address.Parse(addressString), network.Address);
        Assert.AreEqual(prefix, network.Prefix);
        Assert.AreEqual(IPv6Address.Parse(mask), network.Mask);
    }

    private static IEnumerable<object[]> IPv6Network_Parse_Failure_Test_Data() =>
    [
        ["4654yyrehh"],
        ["2001:db8::/129"],
        ["2001:db8::/xx"],
        ["2001:db8::/0"],
        ["2001:db8::/8"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Parse_Failure_Test_Data))]
    public void IPv6Network_Parse_Failure_Test(string networkString)
    {
        Assert.ThrowsExactly<FormatException>(() => IPv6Network.Parse(networkString));
    }

    private static IEnumerable<object[]> IPv6Network_Contains_Test_Data() =>
    [
        ["fec0::/64", "fec0::da94", true],
        ["fec0::/64", "fec1::da94", false]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Contains_Test_Data))]
    public void IPv6Network_Contains_Test(string networkString, string addressString, bool contains)
    {
        var network = IPv6Network.Parse(networkString);
        var address = IPv6Address.Parse(addressString);
        Assert.AreEqual(contains, network.Contains(address), $"{address} is {(contains ? "" : "not ")}in the {network}");
    }

    private static IEnumerable<object[]> IPv6Network_ContainsNetwork_Test_Data() =>
    [
        // Subnet contained in supernet
        ["2001:db8::/32", "2001:db8:1::/48", true],
        ["fec0::/16", "fec0:1::/32", true],
        // Equal networks
        ["fec0::/64", "fec0::/64", true],
        // Supernet NOT contained in subnet
        ["fec0::/64", "fec0::/32", false],
        // Disjoint networks, same prefix
        ["fec0::/64", "fec1::/64", false],
        // Disjoint networks, different prefix
        ["2001:db8::/32", "fe80::/10", false],
        // Host network (/128) contained in larger network
        ["fec0::/64", "fec0::1/128", true],
        // Host network (/128) NOT contained
        ["fec0::/64", "fec1::1/128", false],
        // /128 contains only itself
        ["2001:db8::1/128", "2001:db8::1/128", true],
        ["2001:db8::1/128", "2001:db8::2/128", false],
        // Boundary: subnet at the end of parent range
        ["2001:db8::/32", "2001:db8:ffff::/48", true],
        // Boundary: subnet just outside parent range
        ["2001:db8::/32", "2001:db9::/48", false],
        // /0 contains everything
        ["::/0", "2001:db8::/32", true],
        ["::/0", "fec0::/64", true],
        ["::/0", "::/0", true]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_ContainsNetwork_Test_Data))]
    public void IPv6Network_ContainsNetwork_Test(string outerString, string innerString, bool contains)
    {
        var outer = IPv6Network.Parse(outerString, false);
        var inner = IPv6Network.Parse(innerString, false);
        Assert.AreEqual(contains, outer.Contains(inner), $"{innerString} is {(contains ? "" : "not ")}in {outerString}");
    }

    private static IEnumerable<object[]> IPv6Network_Indexer_Test_Data() =>
    [
        ["fec0::/64", 0u, "fec0::"],
        ["fec0::/64", 123456789u, "fec0::75b:cd15"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Indexer_Test_Data))]
    public void IPv6Network_Indexer_Test(string networkString, uint index, string addressString)
    {
        var network = IPv6Network.Parse(networkString);
        Assert.AreEqual(addressString, network[index].ToString());
    }

    private static IEnumerable<object[]> IPv6Network_Subnet_Test_Data() =>
    [
        ["fec1::/64", 69, 1, "fec1::800:0:0:0/69"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Subnet_Test_Data))]
    public void IPv6Network_Subnet_Test(string networkString, int prefix, int index, string subnetString)
    {
        var network = IPv6Network.Parse(networkString);
        var subnet = network.Subnet((byte)prefix, index);
        Assert.AreEqual(subnetString, subnet.ToString());
    }

    private static IEnumerable<object[]> IPv6Network_Supernet_Test_Data() =>
    [
        ["fec1::800:0:0:0/69", 64, "fec1::/64"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_Supernet_Test_Data))]
    public void IPv6Network_Supernet_Test(string networkString, int prefix, string supernetString)
    {
        var network = IPv6Network.Parse(networkString);
        var supernet = network.Supernet((byte)prefix);
        Assert.AreEqual(supernetString, supernet.ToString());
    }

    private static IEnumerable<object[]> IPv6Network_ToString_Test_Data() =>
    [
        ["fec0::/64"],
        ["2001:db8::/32"],
        ["2001:db8::1/128"]
    ];

    [TestMethod]
    [DynamicData(nameof(IPv6Network_ToString_Test_Data))]
    public void IPv6Network_ToString_Test(string networkString)
    {
        var network = IPv6Network.Parse(networkString);
        Assert.AreEqual(networkString, network.ToString());
    }
}