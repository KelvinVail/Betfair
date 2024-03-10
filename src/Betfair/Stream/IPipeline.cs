namespace Betfair.Stream;

internal interface IPipeline
{
    Task WriteLines(object value, CancellationToken cancellationToken);

    IAsyncEnumerable<byte[]> ReadLines(CancellationToken cancellationToken);
}