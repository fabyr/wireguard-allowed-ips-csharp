using System.Numerics;

namespace WireguardAllowedIPs.Core;

public class IPv4Network : IPNetwork
{
    public static readonly IPv4Network Any = new(new byte[] { 0, 0, 0, 0 }, 0);

    public uint AddressValue => (uint)(
                                    ((uint)AddressBytes![0] << 24) |
                                    ((uint)AddressBytes![1] << 16) |
                                    ((uint)AddressBytes![2] << 8) |
                                    ((uint)AddressBytes![3])
                                );

    public override string AddressRepresentation => string.Join(".", AddressBytes!);

    public IPv4Network(byte[] bytes, int cidr) : base(cidr)
    {
        if(bytes.Length != 4)
            throw new ArgumentException("An IPv4 address always consists of 32 bits (4 bytes)");
        if(cidr < 0 || cidr > 32)
            throw new ArgumentOutOfRangeException("cidr", "CIDR must be between 0 and 32 (both inclusive)");
        AddressBytes = bytes;
    }

    public IPv4Network(uint value, int cidr) : this(new byte[] { 
        (byte)(value >> 24),
        (byte)((value >> 16) & 0xFF),
        (byte)((value >> 8) & 0xFF),
        (byte)(value & 0xFF),
    }, cidr)
    {

    }

    public IPv4Network(string addressString, int cidr) : this(ParseAddressString(addressString), cidr)
    { }

    public uint GetHostMask() => Cidr == 0 ? uint.MaxValue : (1u << (32 - Cidr)) - 1;
    public uint GetNetworkMask() => ~GetHostMask();
    public uint GetLowAddressValue() => AddressValue & GetNetworkMask();
    public uint GetHighAddressValue() => AddressValue | GetHostMask();

    public override bool Contains(IPNetwork other)
    {
        if(other is IPv4Network ip)
        {
            return ip.GetLowAddressValue() >= GetLowAddressValue()
                   && ip.GetHighAddressValue() <= GetHighAddressValue();
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv4Network)}");
    }

    public override bool Overlaps(IPNetwork other)
    {
        if(other is IPv4Network ip)
        {
            return uint.Max(ip.GetLowAddressValue(), GetLowAddressValue()) <= 
                   uint.Min(ip.GetHighAddressValue(), GetHighAddressValue());
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv4Network)}");
    }

    public override IPNetwork[] Exclude(IPNetwork b)
    {
        if(b is IPv4Network ip)
        {   
            if(!Overlaps(ip))
                throw new ArgumentOutOfRangeException("Address ranges don't overlap.");
            int minCidr = Math.Min(Cidr + 1, ip.Cidr);
            int maxCidr = Math.Max(Cidr + 1, ip.Cidr);

            IPv4Network first = this;
            IPv4Network last = ip;
            if(ip.AddressValue < first.AddressValue)
                (first, last) = (last, first);

            return new IPv4Network(first.AddressValue & first.GetNetworkMask(), 32)
                    .SummarizeAddressRangeWith(new IPv4Network(last.AddressValue & last.GetNetworkMask() - 1, 32))
                    .Concat(
                        new IPv4Network(last.AddressValue | last.GetHostMask() + 1, 32)
                        .SummarizeAddressRangeWith(new IPv4Network(first.AddressValue | first.GetNetworkMask(), 32))
                    ).Where(x => x.Cidr >= minCidr && x.Cidr <= maxCidr).ToArray();
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv4Network)}");
    }

    public override IPNetwork[] SummarizeAddressRangeWith(IPNetwork b)
    {
        if(b is IPv4Network ip)
        {
            uint first = AddressValue;
            uint last = ip.AddressValue;
            if(last < first)
                (first, last) = (last, first);
            List<IPv4Network> list = new();
            while(first <= last)
            {
                int nbits = Math.Min(Util.CountRighthandZeroBits32(first), Util.BitLength32(last - first + 1) - 1);
                list.Add(new IPv4Network(first, 32 - nbits));
                try
                {
                    checked 
                    {
                        first += 1u << nbits;
                    }
                } catch(OverflowException)
                {
                    break;
                }
            }
            return list.ToArray();
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv4Network)}");
    }

    public override BigInteger Scalar()
    {
        return new BigInteger(AddressValue);
    }

    public static byte[] ParseAddressString(string addressString)
    {
        string[] parts = addressString.Split('.');
        if(parts.Length != 4)
            throw new FormatException("IPv4 Address must consist of 4 octets.");
        if(parts.Any(x => !byte.TryParse(x, out _)))
            throw new FormatException("Invalid IPv4 octet values.");
        
        return parts.Select(x => byte.Parse(x)).ToArray();
    }

    public override bool Equals(object? obj)
    {
        if(obj is IPv4Network ip)
            return ip.AddressValue == AddressValue && ip.Cidr == Cidr;
        return false;
    }

    public override int GetHashCode()
    { 
        return HashCode.Combine(AddressValue, Cidr);
    }
}