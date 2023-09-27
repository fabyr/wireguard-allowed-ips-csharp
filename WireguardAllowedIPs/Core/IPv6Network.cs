using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace WireguardAllowedIPs.Core;

public class IPv6Network : IPNetwork
{
    public static readonly IPv6Network Any = new(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);

    private enum ParseState
    {
        ReadyForData,
        InSeparator
    }
    
    public override string AddressRepresentation => GetAddressString();

    public UInt128 AddressValue => (UInt128)(
                                    ((UInt128)AddressBytes![0]  << 120) |
                                    ((UInt128)AddressBytes![1]  << 112) |
                                    ((UInt128)AddressBytes![2]  << 104) |
                                    ((UInt128)AddressBytes![3]  << 96) |
                                    ((UInt128)AddressBytes![4]  << 88) |
                                    ((UInt128)AddressBytes![5]  << 80) |
                                    ((UInt128)AddressBytes![6]  << 72) |
                                    ((UInt128)AddressBytes![7]  << 64) |
                                    ((UInt128)AddressBytes![8]  << 56) |
                                    ((UInt128)AddressBytes![9]  << 48) |
                                    ((UInt128)AddressBytes![10] << 40) |
                                    ((UInt128)AddressBytes![11] << 32) |
                                    ((UInt128)AddressBytes![12] << 24) |
                                    ((UInt128)AddressBytes![13] << 16) |
                                    ((UInt128)AddressBytes![14] << 8) |
                                    ((UInt128)AddressBytes![15])
                                );

    public IPv6Network(byte[] bytes, int cidr) : base(cidr)
    {
        if(bytes.Length != 16)
            throw new ArgumentException("An IPv6 address always consists of 128 bits (16 bytes)");
        if(cidr < 0 || cidr > 128)
            throw new ArgumentOutOfRangeException("cidr", "CIDR must be between 0 and 128 (both inclusive)");
        AddressBytes = bytes;
    }

    public IPv6Network(UInt128 value, int cidr) : this(new byte[] { 
        (byte)(value >> 120),
        (byte)((value >> 112) & 0xFF),
        (byte)((value >> 104) & 0xFF),
        (byte)((value >> 96) & 0xFF),
        (byte)((value >> 88) & 0xFF),
        (byte)((value >> 80) & 0xFF),
        (byte)((value >> 72) & 0xFF),
        (byte)((value >> 64) & 0xFF),
        (byte)((value >> 56) & 0xFF),
        (byte)((value >> 48) & 0xFF),
        (byte)((value >> 40) & 0xFF),
        (byte)((value >> 32) & 0xFF),
        (byte)((value >> 24) & 0xFF),
        (byte)((value >> 16) & 0xFF),
        (byte)((value >> 8) & 0xFF),
        (byte)(value & 0xFF)
    }, cidr)
    {

    }

    public IPv6Network(string addressString, int cidr) : this(ParseAddressString(addressString), cidr)
    { }

    public UInt128 GetHostMask() => Cidr == 0 ? UInt128.MaxValue : (UInt128.One << (128 - Cidr)) - 1;
    public UInt128 GetNetworkMask() => ~GetHostMask();
    public UInt128 GetLowAddressValue() => AddressValue & GetNetworkMask();
    public UInt128 GetHighAddressValue() => AddressValue | GetHostMask();

    public override bool Contains(IPNetwork other)
    {
        if(other is IPv6Network ip)
        {
            return ip.GetLowAddressValue() >= GetLowAddressValue()
                   && ip.GetHighAddressValue() <= GetHighAddressValue();
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv6Network)}");
    }

    public override bool Overlaps(IPNetwork other)
    {
        if(other is IPv6Network ip)
        {
            return UInt128.Max(ip.GetLowAddressValue(), GetLowAddressValue()) <= 
                   UInt128.Min(ip.GetHighAddressValue(), GetHighAddressValue());
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv6Network)}");
    }

    public override IPNetwork[] SummarizeAddressRangeWith(IPNetwork other)
    {
        if(other is IPv6Network ip)
        {
            if(Cidr != 128 || ip.Cidr != 128)
                throw new ArgumentException("Can only construct an address range between two /32 addresses");

            UInt128 first = AddressValue;
            UInt128 last = ip.AddressValue;
            if(ip.AddressValue < AddressValue)
                (first, last) = (last, first);
            List<IPv6Network> list = new();
            while(first <= last)
            {
                int nbits = Math.Min(Util.CountRighthandZeroBits128(first), Util.BitLength128(last - first + 1) - 1);
                list.Add(new IPv6Network(first, 128 - nbits));
                try
                {
                    checked 
                    {
                        first += UInt128.One << nbits;
                    }
                } catch(OverflowException)
                {
                    break;
                }
            }
            return list.ToArray();
        }
        throw new ArgumentException("other", $"Can only compare to another instance of {nameof(IPv6Network)}");
    }

    public override bool Equals(object? obj)
    {
        if(obj is IPv6Network ip)
            return ip.AddressValue == AddressValue && ip.Cidr == Cidr;
        return false;
    }

    public override int GetHashCode()
    { 
        return HashCode.Combine(AddressValue, Cidr);
    }

    private string GetAddressString(bool excludeZeroSegments = true)
    {
        StringBuilder sb = new();

        // Transform 16 bytes into 8 x 16-bit segments
        ushort[] segments = new ushort[8];
        for(int i = 0; i < 8; i++)
        {
            ushort value = (ushort)((AddressBytes![i * 2] << 8) | AddressBytes![i * 2 + 1]);
            segments[i] = value;
        }

        List<(int, int)> zeroSections = new();
        
        bool inZeroSection = false;
        int currentCounter = 0;

        // Find all indices at which zeros occur. The second integer indicates the additional zeros afterwards.
        for(int i = 0; i < segments.Length; i++)
        {
            if(segments[i] == 0)
            {
                if(inZeroSection)
                {
                    currentCounter++;
                }
                else
                {
                    currentCounter = 0;
                    inZeroSection = true;
                }
                continue;
            }
            if(inZeroSection)
            {
                zeroSections.Add((i - 1 - currentCounter, currentCounter));
            }
            inZeroSection = false;
        }
        if(inZeroSection)
            zeroSections.Add((segments.Length - 1 - currentCounter, currentCounter));

        // Select the longest continous streak of zeros, which will become '::'
        (int, int)? longest = zeroSections.Count > 0 ? zeroSections.MaxBy((x) => x.Item2) : null;

        bool tillEnd = longest != null && longest.Value.Item1 + longest.Value.Item2 == 7;

        for(int i = 0; i < segments.Length; i++)
        {
            if(longest != null)
            {
                if(longest.Value.Item1 == i)
                {
                    sb.Append(":");
                    continue;
                }
                else if(i > longest.Value.Item1 && i <= longest.Value.Item1 + longest.Value.Item2)
                    continue;
            }
            if(i != 0)
                sb.Append(":");
            sb.AppendFormat("{0:x}", segments[i]);
        }

        // If the last segment is part of a zero-streak, we need to fill up a missing colon
        if(tillEnd)
            sb.Append(":");

        return sb.ToString();
    }
    
    public static byte[] ParseAddressString(string addressString)
    {
        const int maxSegments = 8;

        using StringReader sr = new(addressString);
        int charValue;

        List<string> partA = new();
        List<string> partB = new();

        bool hasSeenDoubleSeparator = false;

        Func<List<string>> getList = () => hasSeenDoubleSeparator ? partB : partA;
        Func<bool> checkSegmentCount = () => partA.Count + partB.Count <= maxSegments;

        StringBuilder buffer = new();

        ParseState state = ParseState.ReadyForData;

        while((charValue = sr.Read()) != -1)
        {
            char character = (char)charValue;
            switch(state)
            {
                case ParseState.ReadyForData:
                {
                    if(char.IsWhiteSpace(character))
                        continue; // Ignore whitespace
                    else if(char.IsAsciiHexDigit(character))
                    {
                        buffer.Append(character);
                    }
                    else if(character == ':')
                    {
                        if(buffer.Length > 0)
                            getList().Add(buffer.ToString()); // We finished the previous segment
                        buffer.Clear();
                        if(!checkSegmentCount())
                            throw new FormatException("Too many segments in IPv6 address.");
                        state = ParseState.InSeparator;
                    }
                    else
                        throw new FormatException("Invalid character in address.");
                } break;
                case ParseState.InSeparator:
                {
                    if(char.IsWhiteSpace(character))
                        continue; // Ignore whitespace
                    else if(character == ':') // we have a double :: separator
                    {
                        if(hasSeenDoubleSeparator)
                            throw new FormatException("Invalid IPv6 address format: Saw '::' twice");
                        hasSeenDoubleSeparator = true;
                    }
                    else if(char.IsAsciiHexDigit(character))
                    {
                        buffer.Append(character); // Add current digit to next segment

                        state = ParseState.ReadyForData;
                    }
                    else
                        throw new FormatException("Invalid character in address.");
                    
                } break;
                default:
                    throw new Exception("This should not happen :c");
            }
        }
        if(buffer.Length > 0)
            getList().Add(buffer.ToString());
        if(!checkSegmentCount())
            throw new FormatException("Too many segments in IPv6 address.");

        int missingSegments = maxSegments - partA.Count - partB.Count;

        List<byte> bytes = new();

        foreach(string value in partA.Concat(Enumerable.Repeat("0000", missingSegments)).Concat(partB))
        {
            if (!ushort.TryParse(value, NumberStyles.HexNumber, null, out ushort v))
                throw new FormatException("Invalid hex value for IPv6 segment.");
            bytes.Add((byte)(v >> 8));
            bytes.Add((byte)(v & 0xFF));
        }

        return bytes.ToArray();
    }
}