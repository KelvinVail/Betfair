using Betfair.Stream;
using Utf8Json;
using Utf8Json.Resolvers;
#pragma warning disable SA1010

namespace Betfair.Tests.Stream.TestDoubles;

public class PipelineStub : IPipeline
{
    public List<object> ObjectsWritten { get; } = [];

    public List<object> ObjectsToBeRead { get; } = [];

    public async Task Write(object value, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        ObjectsWritten.Add(value);
    }

    public async IAsyncEnumerable<byte[]> Read()
    {
        await Task.CompletedTask;
        foreach (var o in ObjectsToBeRead)
            yield return JsonSerializer.Serialize(o, StandardResolver.AllowPrivateExcludeNullCamelCase);
    }
}
