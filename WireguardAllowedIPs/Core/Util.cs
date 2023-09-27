namespace WireguardAllowedIPs.Core;

internal static class Util
{
    /// <summary>
    /// Counts the minimum number of bits needed to represent this 32-bit integer value.
    /// i.e. if a certain number of high bits are zero, the result will be less than 32
    /// </summary>
    /// <param name="value">The value in question</param>
    /// <returns>Number of bits</returns>
    public static int BitLength32(uint value)
    {
        int bitLength = 0;
        while((value >> bitLength) > 0 && bitLength < 32)
        {
            bitLength++;
        }
        return bitLength;
    }

    /// <summary>
    /// Counts the number of zeros after the highest set bit up until 32.
    /// i.e. if a certain number of high bits are zero, the result will be that amount
    /// </summary>
    /// <param name="value">The value in question</param>
    /// <returns>Number of bits</returns>
    public static int CountRighthandZeroBits32(uint value)
    {
        if(value == 0)
            return 32;
        
        return Math.Min(32, BitLength32(~value & (value - 1)));
    }
    
    /// <summary>
    /// Counts the minimum number of bits needed to represent this 128-bit integer value.
    /// i.e. if a certain number of high bits are zero, the result will be less than 128
    /// </summary>
    /// <param name="value">The value in question</param>
    /// <returns>Number of bits</returns>
    public static int BitLength128(UInt128 value)
    {
        int bitLength = 0;
        while((value >> bitLength) > 0 && bitLength < 128)
        {
            bitLength++;
        }
        return bitLength;
    }

    /// <summary>
    /// Counts the number of zeros after the highest set bit up until 128.
    /// i.e. if a certain number of high bits are zero, the result will be that amount
    /// </summary>
    /// <param name="value">The value in question</param>
    /// <returns>Number of bits</returns>
    public static int CountRighthandZeroBits128(UInt128 value)
    {
        if(value == 0)
            return 128;
        
        return Math.Min(128, BitLength128(~value & (value - 1)));
    }
}