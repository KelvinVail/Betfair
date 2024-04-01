namespace Betfair.Benchmarks.Deserializers;

internal class ByteComparer : EqualityComparer<byte[]>
{
    public override bool Equals(byte[]? x, byte[]? y) => x.SequenceEqual(y);

    public override int GetHashCode(byte[] obj)
    {
        int sum = 0;
        foreach ( byte cur in obj )
          sum += cur;

        return sum;
    }
}
