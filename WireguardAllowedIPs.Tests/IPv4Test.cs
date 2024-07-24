namespace WireguardAllowedIPs.Tests;

public class IPv4Test
{
    [Theory]
    [InlineData("127.0.0.0/8")]
    [InlineData("0.0.0.0/0")]
    [InlineData("5.75.0.1/24")]
    [InlineData("105.84.79.196/32")]
    [InlineData("10.0.0.0/16")]
    public void Parse_ToString_Equality(string ipv4)
    {
        IPv4Network ip = (IPv4Network)IPNetwork.Parse(ipv4);

        Assert.Equal(ipv4, ip.ToString());
    }

    [Theory]
    [InlineData("127.0.0.0/8", 0x7F000000u)]
    [InlineData("0.0.0.0/0", 0x00000000u)]
    [InlineData("5.75.0.1/24", 0x054B0001u)]
    [InlineData("105.84.79.196/32", 0x69544FC4u)]
    [InlineData("10.0.0.0/16", 0x0A000000u)]
    public void Address_Value_Test(string ipv4, uint expectedAddressValue)
    {
        IPv4Network ip = (IPv4Network)IPNetwork.Parse(ipv4);

        Assert.Equal(ip.AddressValue, expectedAddressValue);
    }

    [Fact]
    public void Address_SummaryRange_Test1()
    {
        IPv4Network from = new("4.21.11.19", 32);
        IPv4Network to = new("10.0.11.0", 32);

        string[] expected =
        [
            "4.21.11.19/32", "4.21.11.20/30", "4.21.11.24/29", "4.21.11.32/27", "4.21.11.64/26", "4.21.11.128/25", "4.21.12.0/22", "4.21.16.0/20", "4.21.32.0/19", "4.21.64.0/18", "4.21.128.0/17", "4.22.0.0/15", "4.24.0.0/13", "4.32.0.0/11", "4.64.0.0/10", "4.128.0.0/9", "5.0.0.0/8", "6.0.0.0/7", "8.0.0.0/7", "10.0.0.0/21", "10.0.8.0/23", "10.0.10.0/24", "10.0.11.0/32"
        ];

        string[] output = from.SummarizeAddressRangeWith(to).Select(x => x.ToString()).ToArray();

        Assert.True(new HashSet<string>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Address_SummaryRange_Test2()
    {
        IPv4Network from = new("192.168.0.0", 32);
        IPv4Network to = new("255.254.224.1", 32);

        string[] expected =
        [
            "192.168.0.0/13", "192.176.0.0/12", "192.192.0.0/10", "193.0.0.0/8", "194.0.0.0/7", "196.0.0.0/6", "200.0.0.0/5", "208.0.0.0/4", "224.0.0.0/4", "240.0.0.0/5", "248.0.0.0/6", "252.0.0.0/7", "254.0.0.0/8", "255.0.0.0/9", "255.128.0.0/10", "255.192.0.0/11", "255.224.0.0/12", "255.240.0.0/13", "255.248.0.0/14", "255.252.0.0/15", "255.254.0.0/17", "255.254.128.0/18", "255.254.192.0/19", "255.254.224.0/31"
        ];

        string[] output = from.SummarizeAddressRangeWith(to).Select(x => x.ToString()).ToArray();

        Assert.True(new HashSet<string>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }
}