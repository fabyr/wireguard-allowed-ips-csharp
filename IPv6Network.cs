using System.Globalization;
using System.Text;

namespace WireguardAllowedIPs;

public class IPv6Network : IPNetwork
{
    private enum ParseState
    {
        ReadyForData,
        InSeparator
    }

    public byte[] AddressBytes { get; }

    public override string AddressRepresentation => GetAddressString();

    public IPv6Network(byte[] bytes, int cidr) : base(cidr)
    {
        if(bytes.Length != 16)
            throw new ArgumentException("An IPv6 address always consists of 128 bits (16 bytes)");
        if(cidr < 0 || cidr > 128)
            throw new ArgumentOutOfRangeException("cidr", "CIDR must be between 0 and 128 (both inclusive)");
        AddressBytes = bytes;
    }

    public IPv6Network(string addressString, int cidr) : this(ParseAddressString(addressString), cidr)
    { }

    private string GetAddressString(bool excludeZeroSegments = true)
    {
        StringBuilder sb = new();
        bool isWaitingForNonzero = false;
        bool hasUsedDoubleSeparator = false;
        for(int i = 0; i < 8; i++)
        {
            ushort value = (ushort)((AddressBytes[i * 2] << 8) | AddressBytes[i * 2 + 1]);
            if(excludeZeroSegments)
            {
                if(!hasUsedDoubleSeparator && value == 0)
                {
                    if(!isWaitingForNonzero)
                    {
                        sb.Append(":");
                        isWaitingForNonzero = true;
                    }
                    continue;
                }
                if(isWaitingForNonzero && value != 0)
                {
                    hasUsedDoubleSeparator = true;
                }
            }
            
            if(i != 0)
                sb.Append(":");
            sb.AppendFormat("{0:x}", value);
        }
        if(isWaitingForNonzero && !hasUsedDoubleSeparator)
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