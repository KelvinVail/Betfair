#pragma warning disable S3903 // Types should be defined in named namespaces

using System.Threading;

/// <summary>
/// Compatibility shim for xUnit v3's TestContext when running on xUnit v2.
/// </summary>
internal static class TestContext
{
    public static TestContextCurrent Current { get; } = new ();

    internal sealed class TestContextCurrent
    {
#pragma warning disable CA1822, S2325 // Instance access required for xUnit v3 compatibility
        public CancellationToken CancellationToken => CancellationToken.None;
#pragma warning restore CA1822, S2325
    }
}
