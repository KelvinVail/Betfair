using Betfair.Core.Authentication;

namespace Betfair.Core.Client;

internal static class BetfairHttpFactory
{
    internal static HttpAdapter Create(
        Credentials credentials,
        TokenProvider tokenProvider,
        BetfairHttpClient client)
    {
        var deserializer = new HttpDeserializer(client);
        var tokenInjector = new HttpTokenInjector(deserializer, tokenProvider, credentials.AppKey);
        return new HttpAdapter(tokenInjector);
    }
}