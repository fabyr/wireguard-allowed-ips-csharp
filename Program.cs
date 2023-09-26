using System;

namespace WireguardAllowedIPs;

public class Program
{
    public static void Main(string[] args)
    {
        string[] testV4 = new[]
        {
            "210.113.212.175",
            "56.223.79.66",
            "183.92.181.44",
            "137.139.121.78",
            "50.107.30.42",
            "99.209.202.88",
            "19.133.140.20",
            "13.196.116.119",
            "62.209.0.96",
            "80.80.14.203",
        };

        foreach(string t in testV4)
        {
            IPv4Network net = new(t, 32);

            Console.WriteLine($"{net.ToString().Split('/')[0] == t}\t=> {net}");
        }

        Console.WriteLine();

        string[] testV6 = new[]
        {
            "de09:a949:9300:ee29:5bb9:4fbb::96f6",
            "8964:40c1:3df0:6609:bbce:9483:dcfa:4b91",
            "8aea:1da3:ae32:8acd:2f13:f6a6:40b2:f8ab",
            "609c:7241:cf1:f5f:d293:4ba1:943e:189d",
            "e3ee:9c06::118d:bf67:5117:1a72:b39b",
            "::efab",
            "3dbb:b949:4595:f0ab:c9f2:c46c:f2c6:2f8b",
            "d1c7:e722:8b78:64f1:48ed:1377:b74d:dc68",
            "796e:1fff::d270:ad68",
            "2d4e:f601:113a:83e5:7de6:69bf:34bd:ccfe",
            "c7d1:2751:887b:a818:2514:f569:2e25:76ab",
            "a1e0:949e:7771:dd2c:2db2:c395:acaa:37f8",
            "f3fd::",
            "b3e3:fe6e:a147:5cb9:1106:c9f8:402b:9ee1",
            "4e31:b16a:3837:4de5:8489:ecd9:4119:5c63",
        };

        foreach(string t in testV6)
        {
            IPv6Network net = new(t, 128);

            Console.WriteLine($"{net.ToString().Split('/')[0] == t}\t=> {net}");
        }
    }
}