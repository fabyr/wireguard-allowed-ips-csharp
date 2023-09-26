using System.Numerics;

namespace WireguardAllowedIPs.Core;

public abstract class IPNetwork
{
    public static IPNetwork Any<T>() where T : IPNetwork
    {
        if(typeof(T) == typeof(IPv4Network))
            return IPv4Network.Any;
        if(typeof(T) == typeof(IPv6Network))
            return IPv4Network.Any;
        throw new ArgumentException("Invalid type.");
    }

    public int Cidr { get; set; }
    public abstract string AddressRepresentation { get; }

    public byte[]? AddressBytes { get; protected set; }

    public IPNetwork(int cidr)
    {
        Cidr = cidr;
    }

    public abstract BigInteger Scalar();

    public abstract bool Contains(IPNetwork other);
    public abstract bool Overlaps(IPNetwork other);

    public override string ToString()
    {
        return $"{AddressRepresentation}/{Cidr}";
    }

    public static IPNetwork Parse(string value)
    {
        string[] parts = value.Split('/');
        if(parts.Length != 2)
            throw new FormatException($"Invalid address: {value}");
        try
        {
            if(parts[0].Contains(':')) // IPv6
                return new IPv6Network(parts[0], int.Parse(parts[1]));
            return new IPv4Network(parts[0], int.Parse(parts[1]));
        }
        catch(FormatException ex)
        {
            throw new FormatException($"Address parsing error ('{value}'): {ex.Message}", ex);
        }
    }

    public abstract IPNetwork[] Exclude(IPNetwork b);

    public abstract IPNetwork[] SummarizeAddressRangeWith(IPNetwork b);
}