namespace WireguardAllowedIPs;

public class IPv4Network : IPNetwork
{
    public byte[] AddressBytes { get; }

    public override string AddressRepresentation => string.Join(".", AddressBytes);

    public IPv4Network(byte[] bytes, int cidr) : base(cidr)
    {
        if(bytes.Length != 4)
            throw new ArgumentException("An IPv4 address always consists of 32 bits (4 bytes)");
        if(cidr < 0 || cidr > 32)
            throw new ArgumentOutOfRangeException("cidr", "CIDR must be between 0 and 32 (both inclusive)");
        AddressBytes = bytes;
    }

    public IPv4Network(string addressString, int cidr) : this(ParseAddressString(addressString), cidr)
    { }

    public static byte[] ParseAddressString(string addressString)
    {
        string[] parts = addressString.Split('.');
        if(parts.Length != 4)
            throw new FormatException("IPv4 Address must consist of 4 octets.");
        if(parts.Any(x => !byte.TryParse(x, out _)))
            throw new FormatException("Invalid IPv4 octet values.");
        
        return parts.Select(x => byte.Parse(x)).ToArray();
    }
}