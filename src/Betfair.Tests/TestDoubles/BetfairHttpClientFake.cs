using Betfair.Client;
using Betfair.Errors;
using CSharpFunctionalExtensions;

namespace Betfair.Tests.TestDoubles;

public class BetfairHttpClientFake : BetfairHttpClient
{
    public Uri LastUriCalled { get; private set; }

    public override async Task<Result<T, ErrorResult>> Post<T>(
        Uri uri,
        Maybe<object> body,
        string sessionToken,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        LastUriCalled = uri;
        return default;
    }
}
