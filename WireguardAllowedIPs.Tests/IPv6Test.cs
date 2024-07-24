#pragma warning disable CA1861 // Avoid constant arrays as arguments
using System.Globalization;

namespace WireguardAllowedIPs.Tests;

public class IPv6Test
{
    [Theory]
    [InlineData("fe80::/16")]
    [InlineData("::/0")]
    [InlineData("::1/128")]
    [InlineData("c4f1:dea:f260:b2d5::/64")]
    [InlineData("10eb:a194:e96f:faf7:b562:5d0b:f8de:8bfd/128")]
    [InlineData("::9d33:a44:e36:81ff:5964:c3c4:3733/64")]
    [InlineData("3d57:d87:5b31::7ca6:172:7/64")]
    [InlineData("efa5:8fe1:e5b3::7a3a/64")]
    [InlineData("c4f1:ea:f260:b2d5::/64")]
    public void Parse_ToString_Equality(string ipv6)
    {
        IPv6Network ip = (IPv6Network)IPNetwork.Parse(ipv6);

        Assert.Equal(ipv6, ip.ToString());
    }

    [Theory]
    [InlineData("0::0/0")]
    [InlineData("0fef::0/16")]
    [InlineData("c4f1:00ea:f260::/64")]
    [InlineData("5f4a:0f68:4481:5183:9614:abae:4003:0862/128")]
    [InlineData("c4f1:ea:f260:b2d5:0:efde:efde:efde/64")]
    public void Parse_ToString_Inequality(string ipv6)
    {
        IPv6Network ip = (IPv6Network)IPNetwork.Parse(ipv6);

        Assert.NotEqual(ipv6, ip.ToString());
    }

    [Theory]
    [InlineData("fe80::/16", "0xFE800000000000000000000000000000")]
    [InlineData("::/0", "0x00000000000000000000000000000000")]
    [InlineData("::0/0", "0x00000000000000000000000000000000")]
    [InlineData("0::0/0", "0x00000000000000000000000000000000")]
    [InlineData("::1/128", "0x00000000000000000000000000000001")]
    [InlineData("c4f1:dea:f260:b2d5::/64", "0xC4F10DEAF260B2D50000000000000000")]
    [InlineData("10eb:a194:e96f:af7:b562:5d0b:f8de:8bfd/128", "0x10EBA194E96F0AF7B5625D0BF8DE8BFD")]
    [InlineData("::9d33:0a44:0e36:81ff:5964:c3c4:3733/64", "0x00009D330A440E3681FF5964C3C43733")]
    [InlineData("3d57:d87:5b31::7ca6:172:7/64", "0x3D570D875B31000000007CA601720007")]
    [InlineData("efa5:8fe1:e5b3::7a3a/64", "0xEFA58FE1E5B300000000000000007A3A")]
    [InlineData("c4f1:ea:f260:b2d5::/64", "0xC4F100EAF260B2D50000000000000000")]
    public void Address_Value_Test(string ipv4, string expectedHexAddressValue)
    {
        UInt128 expectedAddressValue = UInt128.Parse(expectedHexAddressValue[2..], NumberStyles.HexNumber);

        IPv6Network ip = (IPv6Network)IPNetwork.Parse(ipv4);

        Assert.Equal(ip.AddressValue, expectedAddressValue);
    }

    [Fact]
    public void Address_SummaryRange_Test1()
    {
        IPv6Network from = new("c4f1:dea:f260:b2d5::", 128);
        IPv6Network to = new("efa5:8fe1::", 128);

        IPv6Network[] expected = new[]
        {
            "c4f1:dea:f260:b2d5::/64", "c4f1:dea:f260:b2d6::/63", "c4f1:dea:f260:b2d8::/61", "c4f1:dea:f260:b2e0::/59", "c4f1:dea:f260:b300::/56", "c4f1:dea:f260:b400::/54", "c4f1:dea:f260:b800::/53", "c4f1:dea:f260:c000::/50", "c4f1:dea:f261::/48", "c4f1:dea:f262::/47", "c4f1:dea:f264::/46", "c4f1:dea:f268::/45", "c4f1:dea:f270::/44", "c4f1:dea:f280::/41", "c4f1:dea:f300::/40", "c4f1:dea:f400::/38", "c4f1:dea:f800::/37", "c4f1:deb::/32", "c4f1:dec::/30", "c4f1:df0::/28", "c4f1:e00::/23", "c4f1:1000::/20", "c4f1:2000::/19", "c4f1:4000::/18", "c4f1:8000::/17", "c4f2::/15", "c4f4::/14", "c4f8::/13", "c500::/8", "c600::/7", "c800::/5", "d000::/4", "e000::/5", "e800::/6", "ec00::/7", "ee00::/8", "ef00::/9", "ef80::/11", "efa0::/14", "efa4::/16", "efa5::/17", "efa5:8000::/21", "efa5:8800::/22", "efa5:8c00::/23", "efa5:8e00::/24", "efa5:8f00::/25", "efa5:8f80::/26", "efa5:8fc0::/27", "efa5:8fe0::/32", "efa5:8fe1::/128"
        }.Select(x => (IPv6Network)IPNetwork.Parse(x)).ToArray();

        IPv6Network[] output = Array.ConvertAll(from.SummarizeAddressRangeWith(to), x => (IPv6Network)x);

        Assert.True(new HashSet<IPv6Network>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Address_SummaryRange_Test2()
    {
        IPv6Network from = new("10eb:a194:e96f:af7:b562:5d0b:f8de:8bfd", 128);
        IPv6Network to = new("fe80::ea:f260:b2d5", 128);

        IPv6Network[] expected = new[]
        {
            "10eb:a194:e96f:af7:b562:5d0b:f8de:8bfd/128", "10eb:a194:e96f:af7:b562:5d0b:f8de:8bfe/127", "10eb:a194:e96f:af7:b562:5d0b:f8de:8c00/118", "10eb:a194:e96f:af7:b562:5d0b:f8de:9000/116", "10eb:a194:e96f:af7:b562:5d0b:f8de:a000/115", "10eb:a194:e96f:af7:b562:5d0b:f8de:c000/114", "10eb:a194:e96f:af7:b562:5d0b:f8df:0/112", "10eb:a194:e96f:af7:b562:5d0b:f8e0:0/107", "10eb:a194:e96f:af7:b562:5d0b:f900:0/104", "10eb:a194:e96f:af7:b562:5d0b:fa00:0/103", "10eb:a194:e96f:af7:b562:5d0b:fc00:0/102", "10eb:a194:e96f:af7:b562:5d0c::/94", "10eb:a194:e96f:af7:b562:5d10::/92", "10eb:a194:e96f:af7:b562:5d20::/91", "10eb:a194:e96f:af7:b562:5d40::/90", "10eb:a194:e96f:af7:b562:5d80::/89", "10eb:a194:e96f:af7:b562:5e00::/87", "10eb:a194:e96f:af7:b562:6000::/83", "10eb:a194:e96f:af7:b562:8000::/81", "10eb:a194:e96f:af7:b563::/80", "10eb:a194:e96f:af7:b564::/78", "10eb:a194:e96f:af7:b568::/77", "10eb:a194:e96f:af7:b570::/76", "10eb:a194:e96f:af7:b580::/73", "10eb:a194:e96f:af7:b600::/71", "10eb:a194:e96f:af7:b800::/69", "10eb:a194:e96f:af7:c000::/66", "10eb:a194:e96f:af8::/61", "10eb:a194:e96f:b00::/56", "10eb:a194:e96f:c00::/54", "10eb:a194:e96f:1000::/52", "10eb:a194:e96f:2000::/51", "10eb:a194:e96f:4000::/50", "10eb:a194:e96f:8000::/49", "10eb:a194:e970::/44", "10eb:a194:e980::/41", "10eb:a194:ea00::/39", "10eb:a194:ec00::/38", "10eb:a194:f000::/36", "10eb:a195::/32", "10eb:a196::/31", "10eb:a198::/29", "10eb:a1a0::/27", "10eb:a1c0::/26", "10eb:a200::/23", "10eb:a400::/22", "10eb:a800::/21", "10eb:b000::/20", "10eb:c000::/18", "10ec::/14", "10f0::/12", "1100::/8", "1200::/7", "1400::/6", "1800::/5", "2000::/3", "4000::/2", "8000::/2", "c000::/3", "e000::/4", "f000::/5", "f800::/6", "fc00::/7", "fe00::/9", "fe80::/89", "fe80::80:0:0/90", "fe80::c0:0:0/91", "fe80::e0:0:0/93", "fe80::e8:0:0/95", "fe80::ea:0:0/97", "fe80::ea:8000:0/98", "fe80::ea:c000:0/99", "fe80::ea:e000:0/100", "fe80::ea:f000:0/103", "fe80::ea:f200:0/106", "fe80::ea:f240:0/107", "fe80::ea:f260:0/113", "fe80::ea:f260:8000/115", "fe80::ea:f260:a000/116", "fe80::ea:f260:b000/119", "fe80::ea:f260:b200/121", "fe80::ea:f260:b280/122", "fe80::ea:f260:b2c0/124", "fe80::ea:f260:b2d0/126", "fe80::ea:f260:b2d4/127"
        }.Select(x => (IPv6Network)IPNetwork.Parse(x)).ToArray();

        IPv6Network[] output = Array.ConvertAll(from.SummarizeAddressRangeWith(to), x => (IPv6Network)x);

        Assert.True(new HashSet<IPv6Network>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments
