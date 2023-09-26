namespace WireguardAllowedIPs;

public abstract class IPNetwork
{
    public int Cidr { get; set; }
    public abstract string AddressRepresentation { get; }

    public IPNetwork(int cidr)
    {
        Cidr = cidr;
    }

    public override string ToString()
    {
        return $"{AddressRepresentation}/{Cidr}";
    }
}