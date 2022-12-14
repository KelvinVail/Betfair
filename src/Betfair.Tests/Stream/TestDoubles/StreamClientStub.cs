using Betfair.Errors;
using Betfair.Stream;
using Betfair.Stream.Responses;
using CSharpFunctionalExtensions;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Stream.TestDoubles;

public class StreamClientStub : StreamClient
{
    public StreamClientStub(ITcpClient tcpClient)
    : base(tcpClient)
    {
    }

    public List<object> SentMessages { get; } = new ();

    public object Response { get; set; }

    public override async Task SendLine(object value)
    {
        await Task.CompletedTask;
        SentMessages.Add(value);
    }

    public override async Task<Result<Maybe<T>, ErrorResult>> ReadLine<T>()
    {
        await Task.CompletedTask;
        if (Response is null) return Maybe<T>.None;
        var str = JsonSerializer.ToJsonString(Response, StandardResolver.ExcludeNullCamelCase);
        Response = null;
        return Maybe.From(JsonSerializer.Deserialize<T>(str, StandardResolver.ExcludeNullCamelCase));
    }
}
