namespace Betfair.Stream;

internal interface IPipeline
{
    Task WriteLine(object value);

    IAsyncEnumerable<byte[]> ReadLines(CancellationToken cancellationToken);
}