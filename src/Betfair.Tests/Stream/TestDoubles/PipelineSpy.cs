using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.TestDoubles;

public class PipelineSpy : Pipeline
{
    public PipelineSpy()
        : base(new MemoryStream())
    {
        Responses.Add(new ChangeMessage
        {
            Operation = "connection",
            StatusCode = "SUCCESS",
        });
    }

    public List<object> WrittenObjects { get; } = new ();

    public List<ChangeMessage> Responses { get; } = new ();

    public override Task Write(object value)
    {
        WrittenObjects.Add(value);
        return Task.CompletedTask;
    }

    public override async IAsyncEnumerable<ChangeMessage> Read()
    {
        await Task.CompletedTask;
        foreach (var changeMessage in Responses)
            yield return changeMessage;
    }
}
