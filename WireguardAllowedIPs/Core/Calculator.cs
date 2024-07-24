namespace WireguardAllowedIPs.Core;

public static class Calculator
{
    public static IPNetwork[] CalculateAllowedIPs(IPNetwork[] allowed, IPNetwork[] disallowed)
    {
        IPv4Network[] v4allowed = allowed.OfType<IPv4Network>().ToArray();
        IPv4Network[] v4disallowed = disallowed.OfType<IPv4Network>().ToArray();

        IPv6Network[] v6allowed = allowed.OfType<IPv6Network>().ToArray();
        IPv6Network[] v6disallowed = disallowed.OfType<IPv6Network>().ToArray();

        return CalculateAllowedIPv4s(v4allowed, v4disallowed).Cast<IPNetwork>()
               .Concat(CalculateAllowedIPv6s(v6allowed, v6disallowed))
               .ToArray();
    }

    private static IPNetwork[] CleanAllowedIPs(IPNetwork[] values, IPNetwork[] allowed, IPNetwork[] disallowed)
    {
        List<IPNetwork> result = new(values);

        // Remove all entries which conflict with one or more disallowed ips
        result.RemoveAll(x => disallowed.Any(y => x.Overlaps(y)));

        // Remove all entries which are outside of the underlying AllowedIPs-Ranges
        // Nothing happens here if the underlying range is the entire address space (e.g. 0.0.0.0/0 or ::/0)
        result.RemoveAll(x => allowed.Any(y => !y.Contains(x)));

        // Add the networks which did not conflict with any disallowed ips
        result.AddRange(allowed.Where(x => !disallowed.Any(y => y.Overlaps(x))));

        // Remove duplicates and return
        return result.Distinct().ToArray();
    }

    public static IPv4Network[] CalculateAllowedIPv4s(IPv4Network[] allowed, IPv4Network[] disallowed)
    {
        if (disallowed.Length == 0)
            return allowed;
        List<IPv4Network> result = [];

        // Remove duplicates
        disallowed = disallowed.Distinct().ToArray();

        // Remove disallowed ranges which are already contained within another disallowed range
        // Then sort by ascending address value
        IPv4Network[] sortedDisallowed = [.. disallowed.Where(x => !disallowed.Any(y => !x.Equals(y) && y.Contains(x))).OrderBy(x => x.GetLowAddressValue())];

        IPv4Network last = new(0, 32);

        // Treat the entire address space as a continous numeric range (which it is)
        // and find the sections which are not inside a disallowed range.
        // This can be done sequentially because we sorted the disallowed ranges by ascending address value
        foreach (IPv4Network dis in sortedDisallowed)
        {
            uint low = dis.GetLowAddressValue();
            uint high = dis.GetHighAddressValue();
            result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv4Network(low > 0 ? low - 1 : low, 32)), x => (IPv4Network)x));
            last = new IPv4Network(high < uint.MaxValue ? high + 1 : high, 32);
        }

        // Finally the last section extends to the maximum address value (all 1s)
        result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv4Network(uint.MaxValue, 32)), x => (IPv4Network)x));

        return Array.ConvertAll(CleanAllowedIPs(
            Array.ConvertAll(result.ToArray(), x => (IPNetwork)x),
            Array.ConvertAll(allowed, x => (IPNetwork)x),
            Array.ConvertAll(disallowed, x => (IPNetwork)x)
        ), x => (IPv4Network)x);
    }

    public static IPv6Network[] CalculateAllowedIPv6s(IPv6Network[] allowed, IPv6Network[] disallowed)
    {
        // This is exactly the same as the IPv4 version
        // Maybe this could be condensed into a single method with some clever generics/methods in each class?

        if (disallowed.Length == 0)
            return allowed;
        List<IPv6Network> result = [];

        disallowed = disallowed.Distinct().ToArray();

        IPv6Network[] sortedDisallowed = [.. disallowed.Where(x => !disallowed.Any(y => !x.Equals(y) && y.Contains(x))).OrderBy(x => x.GetLowAddressValue())];

        IPv6Network last = new(UInt128.Zero, 128);

        foreach (IPv6Network dis in sortedDisallowed)
        {
            UInt128 low = dis.GetLowAddressValue();
            UInt128 high = dis.GetHighAddressValue();
            result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv6Network(low > 0 ? low - 1 : low, 128)), x => (IPv6Network)x));
            last = new IPv6Network(high < UInt128.MaxValue ? high + 1 : high, 128);
        }

        result.AddRange(Array.ConvertAll(last.SummarizeAddressRangeWith(new IPv6Network(UInt128.MaxValue, 128)), x => (IPv6Network)x));

        return Array.ConvertAll(CleanAllowedIPs(
            Array.ConvertAll(result.ToArray(), x => (IPNetwork)x),
            Array.ConvertAll(allowed, x => (IPNetwork)x),
            Array.ConvertAll(disallowed, x => (IPNetwork)x)
        ), x => (IPv6Network)x);
    }

    public static IPNetwork[] CalculateAllowedIPs(string[] allowed, string[] disallowed)
        => CalculateAllowedIPs(allowed.Select(IPNetwork.Parse).ToArray(),
                               disallowed.Select(IPNetwork.Parse).ToArray());
}