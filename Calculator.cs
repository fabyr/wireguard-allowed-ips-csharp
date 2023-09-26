using System.Numerics;
using WireguardAllowedIPs.Core;

namespace WireguardAllowedIPs;

public static class Calculator
{
    public static IPNetwork[] CalculateAllowedIPs(IPNetwork[] allowed, IPNetwork[] disallowed)
    {
        IPv4Network[] v4allowed = allowed.OfType<IPv4Network>().ToArray();
        IPv4Network[] v4disallowed = disallowed.OfType<IPv4Network>().ToArray();

        IPv6Network[] v6allowed = allowed.OfType<IPv6Network>().ToArray();
        IPv6Network[] v6disallowed = disallowed.OfType<IPv6Network>().ToArray();

        return CalculateAllowedIPv4s(v4allowed, v4disallowed);

        /*return CalculateAllowedIPs<IPv4Network>(v4allowed, v4disallowed).Cast<IPNetwork>()
               .Concat(CalculateAllowedIPs<IPv6Network>(v6allowed, v6disallowed))
               .ToArray();*/
    }

    public static IPv4Network[] CalculateAllowedIPv4s(IPv4Network[] allowed, IPv4Network[] disallowed)
    {
        List<IPv4Network> result = new();
        
        IPv4Network[] sortedDisallowed = disallowed.OrderBy(x => x.AddressValue).ToArray();

        IPv4Network last = new(0, 32);
        
        foreach(IPv4Network dis in sortedDisallowed)
        {
            result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv4Network(dis.AddressValue | dis.GetHostMask(), 32)), x => (IPv4Network)x));
            last = new IPv4Network((dis.AddressValue | dis.GetHostMask()) + 1, 32);
        }

        result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv4Network(uint.MaxValue, 32)), x => (IPv4Network)x));
        
        result.RemoveAll(x => disallowed.Any(y => y.Overlaps(x)));
        
        List<int> rIndices = new();
        for(int i = 0; i < result.Count; i++)
        {
            for(int j = 0; j < result.Count; j++)
            {
                if(j == i)
                    continue;
                if(result[i].Overlaps(result[j]))
                {
                    rIndices.Add(j);
                }
            }
        }

        foreach(int i in rIndices.OrderByDescending(x => x))
            result.RemoveAt(i);

        return result.ToArray();
    }

    public static T[] CalculateAllowedIPs<T>(T[] allowed, T[] disallowed) where T : IPNetwork
    {
        List<T> result = new();
        result.AddRange(allowed);
        T[] sortedDisallowed = disallowed.OrderBy(x => x.Scalar()).ToArray();

        BigInteger lastScalar = 0;

        foreach(T all in result.ToList())
        {
            foreach(T dis in sortedDisallowed)
            {
                if(all.Overlaps(dis) || all.Contains(dis))
                {
                    result.Remove(all);

                }
            }
        }
        
        return result.ToArray();
    }

    public static IPNetwork[] CalculateAllowedIPs(string[] allowed, string[] disallowed)
        => CalculateAllowedIPs(allowed.Select(IPNetwork.Parse).ToArray(), 
                               disallowed.Select(IPNetwork.Parse).ToArray());
}