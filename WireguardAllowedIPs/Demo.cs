// README demo
using WireguardAllowedIPs.Core;

namespace WireguardAllowedIPs;

internal static class Demo
{
    public static void Run()
    {
        string[] allowedIPs = [
            "0.0.0.0/0",
            "::/0"
        ];
        string[] disallowedIPs = [
            "10.0.0.0/8",
            "192.168.0.0/16",
            "172.16.0.0/12",
            "72.8.99.100/32"
        ];

        // The IP-Types have implementations for ToString
        IPNetwork[] result = Calculator.CalculateAllowedIPs(allowedIPs, disallowedIPs);

        Console.WriteLine($"AllowedIPs = {string.Join<IPNetwork>(",", result)}");
    }
}