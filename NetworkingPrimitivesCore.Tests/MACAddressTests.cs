using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class MACAddressTests
{
    [TestMethod]
    public void MACAddress_Size_Test() => Assert.AreEqual(6, Unsafe.SizeOf<MACAddress>());

    [TestMethod]
    public void MACAddress_Broadcast_Test() => Assert.AreEqual(MACAddress.Parse("ff:ff:ff:ff:ff:ff"), MACAddress.Broadcast);

    private static object[][] Test_Format_MACAddresses() =>
    [
        ["4e:81:c0:3c:d1:4f", new byte[]{0x4E, 0x81, 0xC0, 0x3C, 0xD1, 0x4F}],
        ["f2:00:00:02:20:00", new byte[]{0xF2, 0x00, 0x00, 0x02, 0x20, 0x00}]
    ];

    [TestMethod]
    [DynamicData(nameof(Test_Format_MACAddresses))]
    public void MACAddress_Format_Test(string addressString, byte[] addressBytes)
    {
        var macAddress = new MACAddress(addressBytes);
        Assert.AreEqual(addressString, macAddress.ToString());
        Assert.AreEqual(addressString, macAddress.ToString("n"));
        Assert.AreEqual(addressString.ToUpperInvariant(), macAddress.ToString("N"));
        Assert.AreEqual(addressString.Replace(':', '-'), macAddress.ToString("u"));
        Assert.AreEqual(addressString.Replace(':', '-').ToUpperInvariant(), macAddress.ToString("U"));
    }

    private static object?[][] Test_Parse_MACAddresses() =>
    [
        ["4e:81:c0:3c:d1:4f", "4e:81:c0:3c:d1:4f"],
        ["f2:00:00:02:20:00", "f2:00:00:02:20:00"],
        ["4e-81-c0-3c-d1-4f", "4e:81:c0:3c:d1:4f"],
        ["F2:00:00:D2:2c:eF", "f2:00:00:d2:2c:ef"],
        ["f2:00:00:02:20:0", "f2:00:00:02:20:00"],
        ["f2:0:0:2:20:00", "f2:00:00:02:20:00"],
        ["4e:81:c00:3c:d1:4f", null],
        ["4e:81:c0:3c:d1:", null],
        ["4e:81::c0:3c:d1", null],
        ["4e:81:c00:3c:d1:4f:00", null],
        ["4e_81-c0-3c-d1-4f", null],
        ["f2:00:00:02:20;00", null],
        ["f2:00:0g:02:20:00", null],
        ["not a mac address", null]
    ];

    [TestMethod]
    [DynamicData(nameof(Test_Parse_MACAddresses))]
    public void MACAddress_Parse_Test(string addressString, string? normalizedAddressString)
    {
        var isValid = normalizedAddressString != null;
        Assert.AreEqual(MACAddress.TryParse(addressString, out var address), isValid, $"MAC address {addressString} is expected to be {(isValid ? "valid" : "not valid")}");
        if (isValid)
            Assert.AreEqual(normalizedAddressString, address.ToString());
    }

    [TestMethod]
    [DataRow(false, DisplayName = "Newtonsoft.Json")]
    [DataRow(true, DisplayName = "System.Text.Json")]
    public void MACAddress_Serialization_Test(bool useNewSerializer)
    {
        const string testMACAddressString = "4e:81:c0:3c:d1:4f";
        var testMACAddress = MACAddress.Parse(testMACAddressString);

        var serializedMACAddress = useNewSerializer
            ? System.Text.Json.JsonSerializer.Serialize(testMACAddress)
            : Newtonsoft.Json.JsonConvert.SerializeObject(testMACAddress);
        Assert.AreEqual($"\"{testMACAddressString}\"", serializedMACAddress);

        var deserializedMACAddress = useNewSerializer
            ? System.Text.Json.JsonSerializer.Deserialize<MACAddress>(serializedMACAddress)
            : Newtonsoft.Json.JsonConvert.DeserializeObject<MACAddress>(serializedMACAddress);
        Assert.AreEqual(testMACAddress, deserializedMACAddress);
    }
}