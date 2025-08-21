using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.TestDoubles;

public class PipelineStubWithRetry : IPipeline
{
    public List<object> ObjectsWritten { get; } = [];

    public List<object> ObjectsToBeRead { get; } = [];

    public Queue<ChangeMessage?> StatusResponses { get; } = new ();

    public bool SimulateTimeout { get; set; }

    public bool SimulateTimeoutOnRetry { get; set; }

    public async Task WriteLine(object value)
    {
        await Task.CompletedTask;
        ObjectsWritten.Add(value);

        // Simulate status responses for retry testing
        if (StatusResponses.Count > 0)
        {
            var statusResponse = StatusResponses.Dequeue();
            if (statusResponse != null)
            {
                ObjectsToBeRead.Add(statusResponse);
            }
        }

        // Note: Timeout simulation is only relevant for in-band status waiting
        // When reader is not active, no timeout occurs as messages are written directly
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
