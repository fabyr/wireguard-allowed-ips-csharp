using WireguardAllowedIPs.Core;

namespace WireguardAllowedIPs;

public class Program
{
    public static void Main(string[] args)
    {
        string[] allowed = new[] { "0.0.0.0/0", "::0/0" };
        string[] disallowed = new[] { "10.1.0.0/16", "127.0.0.0/8", "169.254.0.0/16", "172.16.0.0/12", "192.168.0.0/16", "146.70.116.98/32" };

        IPNetwork[] calculated = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Console.WriteLine($"AllowedIPs = {string.Join<IPNetwork>(",", calculated)}");
    }
}