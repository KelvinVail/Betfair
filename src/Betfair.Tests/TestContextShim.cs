using System.Threading;

/// <summary>
/// Compatibility shim for xUnit v3's TestContext when running on xUnit v2.
/// </summary>
internal static class TestContext
{
    public static TestContextCurrent Current { get; } = new ();

    internal sealed class TestContextCurrent
    {
        public CancellationToken CancellationToken => CancellationToken.None;
    }
}
