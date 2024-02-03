namespace Betfair.Stream;

internal interface IPipeline
{
    Task Write(object value, CancellationToken cancellationToken);

    IAsyncEnumerable<byte[]> Read();
}