namespace WireguardAllowedIPs.Core;

public class IPv4Network : IPNetwork
{
    public uint AddressValue => (uint)(
                                    ((uint)AddressBytes![0] << 24) |
                                    ((uint)AddressBytes![1] << 16) |
                                    ((uint)AddressBytes![2] << 8) |
                                    ((uint)AddressBytes![3])
                                );

    public override string AddressRepresentation => string.Join(".", AddressBytes!);

    public IPv4Network(byte[] bytes, int cidr) : base(cidr)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("An IPv4 address always consists of 32 bits (4 bytes)");
        if (cidr < 0 || cidr > 32)
            throw new ArgumentOutOfRangeException(nameof(cidr), "CIDR must be between 0 and 32 (both inclusive)");
        AddressBytes = bytes;
    }

    public IPv4Network(uint value, int cidr) : this([
        (byte)(value >> 24),
        (byte)((value >> 16) & 0xFF),
        (byte)((value >> 8) & 0xFF),
        (byte)(value & 0xFF),
    ], cidr)
    { }

    public IPv4Network(string addressString, int cidr) : this(ParseAddressString(addressString), cidr)
    { }

    public uint GetHostMask() => Cidr == 0 ? uint.MaxValue : (1u << (32 - Cidr)) - 1;
    public uint GetNetworkMask() => ~GetHostMask();
    public uint GetLowAddressValue() => AddressValue & GetNetworkMask();
    public uint GetHighAddressValue() => AddressValue | GetHostMask();

    private static ArgumentException DifferentTypeException() => new("other", $"Can only compare to another instance of {nameof(IPv4Network)}");

    public override bool Contains(IPNetwork other)
    {
        if (other is IPv4Network ip)
        {
            return ip.GetLowAddressValue() >= GetLowAddressValue()
                   && ip.GetHighAddressValue() <= GetHighAddressValue();
        }
        throw DifferentTypeException();
    }

    public override bool Overlaps(IPNetwork other)
    {
        if (other is IPv4Network ip)
        {
            return uint.Max(ip.GetLowAddressValue(), GetLowAddressValue()) <=
                   uint.Min(ip.GetHighAddressValue(), GetHighAddressValue());
        }
        throw DifferentTypeException();
    }

    // Adapted from python sources
    // https://github.com/python/cpython/blob/8ac20e5404127d68624339c0b318abe2d14fe514/Lib/ipaddress.py#L200
    public override IPNetwork[] SummarizeAddressRangeWith(IPNetwork b)
    {
        if (b is IPv4Network ip)
        {
            if (Cidr != 32 || ip.Cidr != 32)
                throw new ArgumentException("Can only construct an address range between two /32 addresses");
            uint first = AddressValue;
            uint last = ip.AddressValue;
            if (last < first)
                (first, last) = (last, first);
            List<IPv4Network> list = [];
            while (first <= last)
            {
                int nbits = Math.Min(Util.CountRighthandZeroBits32(first), Util.BitLength32(last - first + 1) - 1);
                list.Add(new IPv4Network(first, 32 - nbits));
                try
                {
                    checked
                    {
                        first += 1u << nbits;
                    }
                }
                catch (OverflowException)
                {
                    break;
                }
            }
            return [.. list];
        }
        throw DifferentTypeException();
    }

    public static byte[] ParseAddressString(string addressString)
    {
        string[] parts = addressString.Split('.');
        if (parts.Length != 4)
            throw new FormatException("IPv4 Address must consist of 4 octets.");
        if (parts.Any(x => !byte.TryParse(x, out _)))
            throw new FormatException("Invalid IPv4 octet values.");

        return parts.Select(x => byte.Parse(x)).ToArray();
    }

    public override bool Equals(object? obj)
    {
        if (obj is IPv4Network ip)
            return ip.AddressValue == AddressValue && ip.Cidr == Cidr;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AddressValue, Cidr);
    }
}