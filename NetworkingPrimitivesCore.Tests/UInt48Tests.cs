using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingPrimitivesCore.Tests;

[TestClass]
public class UInt48Tests
{
    [TestMethod]
    [DataRow(0x123456789ABCUL)]
    public void UInt48_Numeric_RoundTrip_Test(ulong value)
    {
        Assert.AreEqual(value, (ulong)(UInt48)value);
    }
}