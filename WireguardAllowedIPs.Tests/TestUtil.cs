namespace WireguardAllowedIPs.Tests;

public static class TestUtil
{
    public static void AssertOutput<T>(T[] output, T[] expected)
    {
        Assert.True(new HashSet<T>(expected).SetEquals(output), $"Missing Values: {string.Join(",", expected.Where(x => !output.Contains(x)))}; {string.Join(",", output.Where(x => !expected.Contains(x)))}");
    }
}
