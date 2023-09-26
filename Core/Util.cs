namespace WireguardAllowedIPs.Core;

internal static class Util
{
    public static int BitLength32(uint value)
    {
        int bitLength = 0;
        while((value >> bitLength) > 0 && bitLength < 32)
        {
            bitLength++;
        }
        return bitLength;
    }

    public static int CountRighthandZeroBits32(uint value)
    {
        if(value == 0)
            return 32;
        
        return Math.Min(32, BitLength32(~value & (value - 1)));
    }

    public static int BitLength128(UInt128 value)
    {
        int bitLength = 0;
        while((value >> bitLength) > 0 && bitLength < 128)
        {
            bitLength++;
        }
        return bitLength;
    }

    public static int CountRighthandZeroBits128(UInt128 value)
    {
        if(value == 0)
            return 128;
        
        return Math.Min(128, BitLength128(~value & (value - 1)));
    }
}