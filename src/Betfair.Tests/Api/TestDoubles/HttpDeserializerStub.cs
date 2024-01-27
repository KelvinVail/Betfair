using Betfair.Core.Client;

namespace Betfair.Tests.Api.TestDoubles;

internal class HttpAdapterStub : HttpAdapter
{
    public Uri? LastUriCalled { get; private set; }

    public object? LastContentSent { get; private set; }

    public object? RespondsWithBody { get; set; }

    public override Task<T> PostAsync<T>(Uri uri, object body, CancellationToken ct = default)
    {
        LastUriCalled = uri;
        LastContentSent = body;

        if (RespondsWithBody is not null)
            return Task.FromResult((T)RespondsWithBody);

        return Task.FromResult<T>(default!);
    }
}
