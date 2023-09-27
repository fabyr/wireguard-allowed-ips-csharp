namespace WireguardAllowedIPs.Tests;

public class AllowedIPsTest
{
    [Fact]
    public void Test1()
    {
        string[] allowed = new[] { "10.0.0.0/8", "::0/0" };
        string[] disallowed = new[] { "5.67.8.19/26", "10.0.0.0/24", "67.8.22.11/16" };

        IPNetwork[] expected = new[] { "10.0.1.0/24", "10.0.2.0/23", "10.0.4.0/22", "10.0.8.0/21", "10.0.16.0/20", "10.0.32.0/19", "10.0.64.0/18", "10.0.128.0/17", "10.1.0.0/16", "10.2.0.0/15", "10.4.0.0/14", "10.8.0.0/13", "10.16.0.0/12", "10.32.0.0/11", "10.64.0.0/10", "10.128.0.0/9", "::/0" }
                            .Select(x => IPNetwork.Parse(x)).ToArray();
    
        IPNetwork[] output = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Assert.True(new HashSet<IPNetwork>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Test2()
    {
        string[] allowed = new[] { "0.0.0.0/0", "::0/0" };
        
        string[] disallowed = new[] { "127.0.0.0/8", "10.0.0.0/24", "192.168.0.0/24", "172.16.0.0/12", "169.254.0.0/16", "224.0.0.0/4" };

        IPNetwork[] expected = new[] { "0.0.0.0/5", "8.0.0.0/7", "10.0.1.0/24", "10.0.2.0/23", "10.0.4.0/22", "10.0.8.0/21", "10.0.16.0/20", "10.0.32.0/19", "10.0.64.0/18", "10.0.128.0/17", "10.1.0.0/16", "10.2.0.0/15", "10.4.0.0/14", "10.8.0.0/13", "10.16.0.0/12", "10.32.0.0/11", "10.64.0.0/10", "10.128.0.0/9", "11.0.0.0/8", "12.0.0.0/6", "16.0.0.0/4", "32.0.0.0/3", "64.0.0.0/3", "96.0.0.0/4", "112.0.0.0/5", "120.0.0.0/6", "124.0.0.0/7", "126.0.0.0/8", "128.0.0.0/3", "160.0.0.0/5", "168.0.0.0/8", "169.0.0.0/9", "169.128.0.0/10", "169.192.0.0/11", "169.224.0.0/12", "169.240.0.0/13", "169.248.0.0/14", "169.252.0.0/15", "169.255.0.0/16", "170.0.0.0/7", "172.0.0.0/12", "172.32.0.0/11", "172.64.0.0/10", "172.128.0.0/9", "173.0.0.0/8", "174.0.0.0/7", "176.0.0.0/4", "192.0.0.0/9", "192.128.0.0/11", "192.160.0.0/13", "192.168.1.0/24", "192.168.2.0/23", "192.168.4.0/22", "192.168.8.0/21", "192.168.16.0/20", "192.168.32.0/19", "192.168.64.0/18", "192.168.128.0/17", "192.169.0.0/16", "192.170.0.0/15", "192.172.0.0/14", "192.176.0.0/12", "192.192.0.0/10", "193.0.0.0/8", "194.0.0.0/7", "196.0.0.0/6", "200.0.0.0/5", "208.0.0.0/4", "240.0.0.0/4", "::/0" }
                               .Select(x => IPNetwork.Parse(x)).ToArray();
    
        IPNetwork[] output = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Assert.True(new HashSet<IPNetwork>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Test3()
    {
        string[] allowed = new[] { "0.0.0.0/0", "::0/0" };
        
        string[] disallowed = new[] { "192.168.0.0/24", "2.228.149.161/32" };

        IPNetwork[] expected = new[] { "0.0.0.0/7", "2.0.0.0/9", "2.128.0.0/10", "2.192.0.0/11", "2.224.0.0/14", "2.228.0.0/17", "2.228.128.0/20", "2.228.144.0/22", "2.228.148.0/24", "2.228.149.0/25", "2.228.149.128/27", "2.228.149.160/32", "2.228.149.162/31", "2.228.149.164/30", "2.228.149.168/29", "2.228.149.176/28", "2.228.149.192/26", "2.228.150.0/23", "2.228.152.0/21", "2.228.160.0/19", "2.228.192.0/18", "2.229.0.0/16", "2.230.0.0/15", "2.232.0.0/13", "2.240.0.0/12", "3.0.0.0/8", "4.0.0.0/6", "8.0.0.0/5", "16.0.0.0/4", "32.0.0.0/3", "64.0.0.0/2", "128.0.0.0/2", "192.0.0.0/9", "192.128.0.0/11", "192.160.0.0/13", "192.168.1.0/24", "192.168.2.0/23", "192.168.4.0/22", "192.168.8.0/21", "192.168.16.0/20", "192.168.32.0/19", "192.168.64.0/18", "192.168.128.0/17", "192.169.0.0/16", "192.170.0.0/15", "192.172.0.0/14", "192.176.0.0/12", "192.192.0.0/10", "193.0.0.0/8", "194.0.0.0/7", "196.0.0.0/6", "200.0.0.0/5", "208.0.0.0/4", "224.0.0.0/3", "::/0" }
                            .Select(x => IPNetwork.Parse(x)).ToArray();
        
        IPNetwork[] output = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Assert.True(new HashSet<IPNetwork>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Test4()
    {
        string[] allowed = new[] { "0.0.0.0/0", "::0/0" };
        
        string[] disallowed = new[] { "192.168.0.0/24", "2.228.149.161/32", "fe80::ea:f260:b2d0/124" };

        IPNetwork[] expected = new[] { " 0.0.0.0/7", "2.0.0.0/9", "2.128.0.0/10", "2.192.0.0/11", "2.224.0.0/14", "2.228.0.0/17", "2.228.128.0/20", "2.228.144.0/22", "2.228.148.0/24", "2.228.149.0/25", "2.228.149.128/27", "2.228.149.160/32", "2.228.149.162/31", "2.228.149.164/30", "2.228.149.168/29", "2.228.149.176/28", "2.228.149.192/26", "2.228.150.0/23", "2.228.152.0/21", "2.228.160.0/19", "2.228.192.0/18", "2.229.0.0/16", "2.230.0.0/15", "2.232.0.0/13", "2.240.0.0/12", "3.0.0.0/8", "4.0.0.0/6", "8.0.0.0/5", "16.0.0.0/4", "32.0.0.0/3", "64.0.0.0/2", "128.0.0.0/2", "192.0.0.0/9", "192.128.0.0/11", "192.160.0.0/13", "192.168.1.0/24", "192.168.2.0/23", "192.168.4.0/22", "192.168.8.0/21", "192.168.16.0/20", "192.168.32.0/19", "192.168.64.0/18", "192.168.128.0/17", "192.169.0.0/16", "192.170.0.0/15", "192.172.0.0/14", "192.176.0.0/12", "192.192.0.0/10", "193.0.0.0/8", "194.0.0.0/7", "196.0.0.0/6", "200.0.0.0/5", "208.0.0.0/4", "224.0.0.0/3", "::/1", "8000::/2", "c000::/3", "e000::/4", "f000::/5", "f800::/6", "fc00::/7", "fe00::/9", "fe80::/89", "fe80::80:0:0/90", "fe80::c0:0:0/91", "fe80::e0:0:0/93", "fe80::e8:0:0/95", "fe80::ea:0:0/97", "fe80::ea:8000:0/98", "fe80::ea:c000:0/99", "fe80::ea:e000:0/100", "fe80::ea:f000:0/103", "fe80::ea:f200:0/106", "fe80::ea:f240:0/107", "fe80::ea:f260:0/113", "fe80::ea:f260:8000/115", "fe80::ea:f260:a000/116", "fe80::ea:f260:b000/119", "fe80::ea:f260:b200/121", "fe80::ea:f260:b280/122", "fe80::ea:f260:b2c0/124", "fe80::ea:f260:b2e0/123", "fe80::ea:f260:b300/120", "fe80::ea:f260:b400/118", "fe80::ea:f260:b800/117", "fe80::ea:f260:c000/114", "fe80::ea:f261:0/112", "fe80::ea:f262:0/111", "fe80::ea:f264:0/110", "fe80::ea:f268:0/109", "fe80::ea:f270:0/108", "fe80::ea:f280:0/105", "fe80::ea:f300:0/104", "fe80::ea:f400:0/102", "fe80::ea:f800:0/101", "fe80::eb:0:0/96", "fe80::ec:0:0/94", "fe80::f0:0:0/92", "fe80::100:0:0/88", "fe80::200:0:0/87", "fe80::400:0:0/86", "fe80::800:0:0/85", "fe80::1000:0:0/84", "fe80::2000:0:0/83", "fe80::4000:0:0/82", "fe80::8000:0:0/81", "fe80::1:0:0:0/80", "fe80::2:0:0:0/79", "fe80::4:0:0:0/78", "fe80::8:0:0:0/77", "fe80::10:0:0:0/76", "fe80::20:0:0:0/75", "fe80::40:0:0:0/74", "fe80::80:0:0:0/73", "fe80::100:0:0:0/72", "fe80::200:0:0:0/71", "fe80::400:0:0:0/70", "fe80::800:0:0:0/69", "fe80::1000:0:0:0/68", "fe80::2000:0:0:0/67", "fe80::4000:0:0:0/66", "fe80::8000:0:0:0/65", "fe80:0:0:1::/64", "fe80:0:0:2::/63", "fe80:0:0:4::/62", "fe80:0:0:8::/61", "fe80:0:0:10::/60", "fe80:0:0:20::/59", "fe80:0:0:40::/58", "fe80:0:0:80::/57", "fe80:0:0:100::/56", "fe80:0:0:200::/55", "fe80:0:0:400::/54", "fe80:0:0:800::/53", "fe80:0:0:1000::/52", "fe80:0:0:2000::/51", "fe80:0:0:4000::/50", "fe80:0:0:8000::/49", "fe80:0:1::/48", "fe80:0:2::/47", "fe80:0:4::/46", "fe80:0:8::/45", "fe80:0:10::/44", "fe80:0:20::/43", "fe80:0:40::/42", "fe80:0:80::/41", "fe80:0:100::/40", "fe80:0:200::/39", "fe80:0:400::/38", "fe80:0:800::/37", "fe80:0:1000::/36", "fe80:0:2000::/35", "fe80:0:4000::/34", "fe80:0:8000::/33", "fe80:1::/32", "fe80:2::/31", "fe80:4::/30", "fe80:8::/29", "fe80:10::/28", "fe80:20::/27", "fe80:40::/26", "fe80:80::/25", "fe80:100::/24", "fe80:200::/23", "fe80:400::/22", "fe80:800::/21", "fe80:1000::/20", "fe80:2000::/19", "fe80:4000::/18", "fe80:8000::/17", "fe81::/16", "fe82::/15", "fe84::/14", "fe88::/13", "fe90::/12", "fea0::/11", "fec0::/10", "ff00::/8" }
                               .Select(x => IPNetwork.Parse(x)).ToArray();
    
        IPNetwork[] output = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Assert.True(new HashSet<IPNetwork>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }

    [Fact]
    public void Test5()
    {
        string[] allowed = new[] { "10.0.0.0/8", "fe80::0/30" };
        
        string[] disallowed = new[] { "192.168.0.0/24", "2.228.149.161/32", "fe80::ea:f260:b2d0/124" };

        IPNetwork[] expected = new[] { "10.0.0.0/8", "fe80::/89", "fe80::80:0:0/90", "fe80::c0:0:0/91", "fe80::e0:0:0/93", "fe80::e8:0:0/95", "fe80::ea:0:0/97", "fe80::ea:8000:0/98", "fe80::ea:c000:0/99", "fe80::ea:e000:0/100", "fe80::ea:f000:0/103", "fe80::ea:f200:0/106", "fe80::ea:f240:0/107", "fe80::ea:f260:0/113", "fe80::ea:f260:8000/115", "fe80::ea:f260:a000/116", "fe80::ea:f260:b000/119", "fe80::ea:f260:b200/121", "fe80::ea:f260:b280/122", "fe80::ea:f260:b2c0/124", "fe80::ea:f260:b2e0/123", "fe80::ea:f260:b300/120", "fe80::ea:f260:b400/118", "fe80::ea:f260:b800/117", "fe80::ea:f260:c000/114", "fe80::ea:f261:0/112", "fe80::ea:f262:0/111", "fe80::ea:f264:0/110", "fe80::ea:f268:0/109", "fe80::ea:f270:0/108", "fe80::ea:f280:0/105", "fe80::ea:f300:0/104", "fe80::ea:f400:0/102", "fe80::ea:f800:0/101", "fe80::eb:0:0/96", "fe80::ec:0:0/94", "fe80::f0:0:0/92", "fe80::100:0:0/88", "fe80::200:0:0/87", "fe80::400:0:0/86", "fe80::800:0:0/85", "fe80::1000:0:0/84", "fe80::2000:0:0/83", "fe80::4000:0:0/82", "fe80::8000:0:0/81", "fe80::1:0:0:0/80", "fe80::2:0:0:0/79", "fe80::4:0:0:0/78", "fe80::8:0:0:0/77", "fe80::10:0:0:0/76", "fe80::20:0:0:0/75", "fe80::40:0:0:0/74", "fe80::80:0:0:0/73", "fe80::100:0:0:0/72", "fe80::200:0:0:0/71", "fe80::400:0:0:0/70", "fe80::800:0:0:0/69", "fe80::1000:0:0:0/68", "fe80::2000:0:0:0/67", "fe80::4000:0:0:0/66", "fe80::8000:0:0:0/65", "fe80:0:0:1::/64", "fe80:0:0:2::/63", "fe80:0:0:4::/62", "fe80:0:0:8::/61", "fe80:0:0:10::/60", "fe80:0:0:20::/59", "fe80:0:0:40::/58", "fe80:0:0:80::/57", "fe80:0:0:100::/56", "fe80:0:0:200::/55", "fe80:0:0:400::/54", "fe80:0:0:800::/53", "fe80:0:0:1000::/52", "fe80:0:0:2000::/51", "fe80:0:0:4000::/50", "fe80:0:0:8000::/49", "fe80:0:1::/48", "fe80:0:2::/47", "fe80:0:4::/46", "fe80:0:8::/45", "fe80:0:10::/44", "fe80:0:20::/43", "fe80:0:40::/42", "fe80:0:80::/41", "fe80:0:100::/40", "fe80:0:200::/39", "fe80:0:400::/38", "fe80:0:800::/37", "fe80:0:1000::/36", "fe80:0:2000::/35", "fe80:0:4000::/34", "fe80:0:8000::/33", "fe80:1::/32", "fe80:2::/31" }
                               .Select(x => IPNetwork.Parse(x)).ToArray();
    
        IPNetwork[] output = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Assert.True(new HashSet<IPNetwork>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }
}