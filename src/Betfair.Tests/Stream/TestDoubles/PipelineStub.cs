using Betfair.Stream;

namespace Betfair.Tests.Stream.TestDoubles;

public class PipelineStub : IPipeline
{
    public List<object> ObjectsWritten { get; } = [];

    public async Task Write(object value, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        ObjectsWritten.Add(value);
    }

    public IAsyncEnumerable<byte[]> Read() => throw new NotImplementedException();
}
