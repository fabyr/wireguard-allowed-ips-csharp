using WireguardAllowedIPs.Core;

namespace WireguardAllowedIPs;

public class Program
{
    public static void Main(string[] args)
    {
        static void PrintHelp()
        {
            Console.WriteLine("Usage: wireguard-allowed-ips [-h] [-a \"all\"|allowed-ranges] [-d disallowed-ranges]");
            Console.WriteLine();
            Console.WriteLine("Calculate the allowed ranges from an underlying allowed range");
            Console.WriteLine("and disallowed ranges within that range.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine(" -h, --help\t\tDisplays this help.");
            Console.WriteLine();
            Console.WriteLine(" -a, --allowed\t\tSet the allowed ip ranges in CIDR notation, separated by commas.");
            Console.WriteLine("\t\t\tDefault Value if omitted: All IPs are allowed (both IPv4 and IPv6).");
            Console.WriteLine();
            Console.WriteLine(" -d, --disallowed\tSet the disallowed ip ranges in CIDR notation, separated by commas.");
        }

        if(args.Length == 0)
        {
            PrintHelp();
            return;
        }

        string[] allowedIps = new string[] { };
        string[] disallowedIps = new string[] { };

        for(int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if(arg.StartsWith("-"))
            {
                string[] innerArgList;
                if(arg.StartsWith("--"))
                    innerArgList = new string[1] { arg[2..] };
                else
                    // a single dash corresponds to single character arguments afterwards
                    innerArgList = arg[1..].Select(x => x.ToString()).ToArray();
                
                foreach(string innerArg in innerArgList)
                {
                    switch(innerArg)
                    {
                        case "help" or "h":
                            PrintHelp();
                            return;
                        case "allowed" or "a":
                            if(i == args.Length - 1)
                            {
                                Console.WriteLine($"Missing allowed IPs argument! (after '{innerArg}')");
                                return;
                            }
                            allowedIps = args[++i].Split(',');
                            break;
                        case "disallowed" or "d":
                            if(i == args.Length - 1)
                            {
                                Console.WriteLine($"Missing disallowed IPs argument! (after '{innerArg}')");
                                return;
                            }
                            disallowedIps = args[++i].Split(',');
                            break;
                        default:
                            Console.WriteLine($"Unknown argument '{innerArg}'. Use --help for help.");
                            return;
                    }
                }
            }
        }
        
        if(allowedIps.Length == 0)
            allowedIps = new[] { "all" }; // Default is allow all

        if(allowedIps.Length == 1 && allowedIps[0] == "all")
            allowedIps = new[] { "0.0.0.0/0", "::/0" };
        
        IPNetwork[] allowed = new IPNetwork[allowedIps.Length];
        IPNetwork[] disallowed = new IPNetwork[disallowedIps.Length];

        for(int i = 0; i < allowed.Length; i++)
        {
            try
            {
                allowed[i] = IPNetwork.Parse(allowedIps[i]);
            } catch(Exception ex)
            {
                Console.WriteLine($"Could not parse network '{allowedIps[i]}': {ex.Message}");
                return;
            }
        }

        for(int i = 0; i < disallowed.Length; i++)
        {
            try
            {
                disallowed[i] = IPNetwork.Parse(disallowedIps[i]);
            } catch(Exception ex)
            {
                Console.WriteLine($"Could not parse network '{disallowedIps[i]}': {ex.Message}");
                return;
            }
        }

        IPNetwork[] result = Calculator.CalculateAllowedIPs(allowed, disallowed);
        Console.WriteLine(string.Join<IPNetwork>(", ", result));
    }
}