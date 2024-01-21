using Betfair.Core.Client;

namespace Betfair.Tests.Api.TestDoubles;

public class BetfairClientStub : BetfairClient
{
    public Uri? LastUriCalled { get; private set; }

    public object? LastContentSent { get; private set; }

    public object? RespondsWithBody { get; set; }

    internal override Task<T> Post<T>(Uri uri, object? body = null, CancellationToken cancellationToken = default)
    {
        LastUriCalled = uri;
        LastContentSent = body;

        if (RespondsWithBody is not null)
            return Task.FromResult((T)RespondsWithBody);

        return Task.FromResult<T>(default!);
    }
}
