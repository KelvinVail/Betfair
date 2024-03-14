using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream;
#pragma warning disable SA1010

namespace Betfair.Tests.Stream.TestDoubles;

public class PipelineStub : IPipeline
{
    public List<object> ObjectsWritten { get; } = [];

    public List<object> ObjectsToBeRead { get; } = [];

    public async Task WriteLine(object value)
    {
        await Task.CompletedTask;

        ObjectsWritten.Add(value);
    }

    public async IAsyncEnumerable<byte[]> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        foreach (var o in ObjectsToBeRead)
        {
            var serializeToUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(o, o.GetContext());
            yield return serializeToUtf8Bytes;
        }
    }
}
