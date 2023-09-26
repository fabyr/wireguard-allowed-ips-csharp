using WireguardAllowedIPs.Core;

namespace WireguardAllowedIPs;

public class Program
{
    public static void Main(string[] args)
    {
        /*do 
        {
            Console.Write("Enter allowed: ");
            string[]? two = Console.ReadLine()?.Split(' ');
            if(two == null)
                continue;
            
            IPNetwork a = IPNetwork.Parse(two[0]);
            IPNetwork b = IPNetwork.Parse(two[1]);

            Console.WriteLine($"Overlaps? {a.Overlaps(b)}");
            //Console.WriteLine(string.Join(",", .OrderBy(x => x.Cidr)));
        } while(true);*/
        string test = "0.0.0.0/5, 8.0.0.0/7, 10.0.0.0/16, 10.2.0.0/15, 10.4.0.0/14, 10.8.0.0/13, 10.16.0.0/12, 10.32.0.0/11, 10.64.0.0/10, 10.128.0.0/9, 11.0.0.0/8, 12.0.0.0/6, 16.0.0.0/4, 32.0.0.0/3, 64.0.0.0/3, 96.0.0.0/4, 112.0.0.0/5, 120.0.0.0/6, 124.0.0.0/7, 126.0.0.0/8, 128.0.0.0/4, 144.0.0.0/7, 146.0.0.0/10, 146.64.0.0/14, 146.68.0.0/15, 146.70.0.0/18, 146.70.64.0/19, 146.70.96.0/20, 146.70.112.0/22, 146.70.116.0/26, 146.70.116.64/27, 146.70.116.96/31, 146.70.116.99/32, 146.70.116.100/30, 146.70.116.104/29, 146.70.116.112/28, 146.70.116.128/25, 146.70.117.0/24, 146.70.118.0/23, 146.70.120.0/21, 146.70.128.0/17, 146.71.0.0/16, 146.72.0.0/13, 146.80.0.0/12, 146.96.0.0/11, 146.128.0.0/9, 147.0.0.0/8, 148.0.0.0/6, 152.0.0.0/5, 160.0.0.0/5, 168.0.0.0/8, 169.0.0.0/9, 169.128.0.0/10, 169.192.0.0/11, 169.224.0.0/12, 169.240.0.0/13, 169.248.0.0/14, 169.252.0.0/15, 169.255.0.0/16, 170.0.0.0/7, 172.0.0.0/12, 172.32.0.0/11, 172.64.0.0/10, 172.128.0.0/9, 173.0.0.0/8, 174.0.0.0/7, 176.0.0.0/4, 192.0.0.0/9, 192.128.0.0/11, 192.160.0.0/13, 192.169.0.0/16, 192.170.0.0/15, 192.172.0.0/14, 192.176.0.0/12, 192.192.0.0/10, 193.0.0.0/8, 194.0.0.0/7, 196.0.0.0/6, 200.0.0.0/5, 208.0.0.0/4, 224.0.0.0/3, ::/0";
        string pytest = "::/0, 64.0.0.0/2, 192.0.0.0/2, 128.0.0.0/2, 32.0.0.0/3, 64.0.0.0/3, 224.0.0.0/3, 128.0.0.0/3, 160.0.0.0/3, 16.0.0.0/4, 208.0.0.0/4, 96.0.0.0/4, 176.0.0.0/4, 128.0.0.0/4, 0.0.0.0/5, 160.0.0.0/5, 112.0.0.0/5, 152.0.0.0/5, 200.0.0.0/5, 12.0.0.0/6, 120.0.0.0/6, 196.0.0.0/6, 148.0.0.0/6, 8.0.0.0/7, 174.0.0.0/7, 124.0.0.0/7, 194.0.0.0/7, 144.0.0.0/7, 170.0.0.0/7, 11.0.0.0/8, 168.0.0.0/8, 193.0.0.0/8, 126.0.0.0/8, 147.0.0.0/8, 173.0.0.0/8, 10.128.0.0/9, 192.0.0.0/9, 169.0.0.0/9, 146.128.0.0/9, 172.128.0.0/9, 10.64.0.0/10, 192.192.0.0/10, 169.128.0.0/10, 172.64.0.0/10, 146.0.0.0/10, 10.32.0.0/11, 169.192.0.0/11, 192.128.0.0/11, 146.96.0.0/11, 172.32.0.0/11, 10.16.0.0/12, 169.224.0.0/12, 192.176.0.0/12, 146.80.0.0/12, 172.0.0.0/12, 10.8.0.0/13, 169.240.0.0/13, 192.160.0.0/13, 146.72.0.0/13, 10.4.0.0/14, 192.172.0.0/14, 146.64.0.0/14, 169.248.0.0/14, 146.68.0.0/15, 169.252.0.0/15, 10.2.0.0/15, 192.170.0.0/15, 192.169.0.0/16, 146.71.0.0/16, 10.0.0.0/16, 169.255.0.0/16, 146.70.128.0/17, 146.70.0.0/18, 146.70.64.0/19, 146.70.96.0/20, 146.70.120.0/21, 146.70.112.0/22, 146.70.118.0/23, 146.70.117.0/24, 146.70.116.128/25, 146.70.116.0/26, 146.70.116.64/27, 146.70.116.112/28, 146.70.116.104/29, 146.70.116.100/30, 146.70.116.96/31, 146.70.116.99/32";
        IPNetwork[] testValues = test.Split(',').Select(x => IPNetwork.Parse(x)).OrderBy(x => x.Cidr).ThenBy(x => x.AddressRepresentation).ToArray();
        IPNetwork[] testValuesPy = pytest.Split(',').Select(x => IPNetwork.Parse(x)).OrderBy(x => x.Cidr).ThenBy(x => x.AddressRepresentation).ToArray();
        do 
        {
            //Console.Write("Enter allowed: ");
            /*string[]? a = Console.ReadLine()?.Split(' ');
            Console.Write("Enter disallowed: ");
            string[]? d = Console.ReadLine()?.Split(' ');
            if(a == null || d == null)
                continue;*/

            string[] a = "0.0.0.0/0 ::0/0".Split(' ');
            string[] d = "10.1.0.0/16 127.0.0.0/8 169.254.0.0/16 172.16.0.0/12 192.168.0.0/16 146.70.116.98/32".Split(' ');
            
            IPNetwork[] result = Calculator.CalculateAllowedIPs(a, d);
            Console.WriteLine();
            Console.WriteLine(string.Join(",", result.OrderBy(x => x.Cidr).ThenBy(x => x.AddressRepresentation)));
            //Console.WriteLine();
            //Console.WriteLine(string.Join<IPNetwork>(",", testValuesPy));
            Console.WriteLine();
            Console.WriteLine(string.Join<IPNetwork>(",", testValues));

            System.Console.WriteLine();
            System.Console.WriteLine($"EXCEPT A: {string.Join(",", result.Where(x => !testValues.Contains(x)))}");
            System.Console.WriteLine($"EXCEPT B: {string.Join(",", testValues.Where(x => !result.Contains(x)))}");
            return;
        } while(true);

        string[] allowed = new[] { "0.0.0.0/0", "::0/0" };
        string[] disallowed = new[] { "10.1.0.0/16", "127.0.0.0/8", "169.254.0.0/16", "172.16.0.0/12", "192.168.0.0/16", "146.70.116.98/32" };

        IPNetwork[] calculated = Calculator.CalculateAllowedIPs(allowed, disallowed);

        Console.WriteLine($"AllowedIPs = {string.Join<IPNetwork>(",", calculated)}");
    }
}