namespace WireguardAllowedIPs.Core;

/// <summary>
/// Baseclass for IPv4Network and IPv6Network
/// </summary>
public abstract class IPNetwork
{
    public int Cidr { get; set; }
    public abstract string AddressRepresentation { get; }
    public byte[]? AddressBytes { get; protected set; }

    public IPNetwork(int cidr)
    {
        Cidr = cidr;
    }

    /// <summary>
    /// Tests if the current instance fully contains another network. <br/>
    /// (If all addresses in the other network fall within this network.) <br/>
    /// Only addresses of the same type (IPv4, IPv6) may be compared to each other.
    /// </summary>
    /// <param name="other">The other network to compare with.</param>
    /// <returns>True, if <paramref name="other"/> is completely contained within this network. False otherwise.</returns>
    public abstract bool Contains(IPNetwork other);

    /// <summary>
    /// Tests if the current instance overlaps with another network. <br/>
    /// (If any address in the network falls within the other.) <br/>
    /// Only addresses of the same type (IPv4, IPv6) may be compared to each other.
    /// </summary>
    /// <param name="other">The other network to compare with.</param>
    /// <returns>True, if they overlap. False otherwise.</returns>
    public abstract bool Overlaps(IPNetwork other);
    
    /// <summary>
    /// Finds all address ranges (networks) between the address of the current instance and <paramref name="b"/> <br/>
    /// Note: Both addresses must be pure addresses and not networks. <br/>
    /// (The netmask must match the bitlength of the address type, 32 for IPv4 and 128 for IPv6)
    /// </summary>
    /// <param name="b">The higher address to move towards.</param>
    /// <returns>An array of IPNetwork-Instances which describe all the address 
    /// ranges between this instance and <paramref name="b"/></returns>
    public abstract IPNetwork[] SummarizeAddressRangeWith(IPNetwork b);

    /// <summary>
    /// Returns a string with both the address part and the netmask in CIDR notation according to the following format: <br/><br/>
    /// {address}/{cidr}
    /// </summary>
    public override string ToString()
    {
        return $"{AddressRepresentation}/{Cidr}";
    }

    /// <summary>
    /// Parses an IPv4 or IPv6 Address in CIDR notation.<br/>
    /// Example inputs: 192.168.1.1/24 or fe80::/64
    /// </summary>
    /// <param name="value">The string to parse</param>
    /// <returns>Either an instance of <see cref="IPv4Network"/> or <see cref="IPv6Network"/> 
    /// containing the information obtained from the string.</returns>
    /// <exception cref="FormatException"></exception>
    public static IPNetwork Parse(string value)
    {
        string[] parts = value.Split('/');
        if(parts.Length != 2)
            throw new FormatException("Invalid address.");
        if(!int.TryParse(parts[1], out int cidr))
            throw new FormatException("Invalid CIDR.");
        if(parts[0].Contains(':')) // IPv6
            return new IPv6Network(parts[0], cidr);
        return new IPv4Network(parts[0], cidr);
    }
}