using Betfair.Core.Login;

namespace Betfair.Core.Client;

internal static class BetfairHttpFactory
{
    internal static HttpAdapter Create(Credentials credentials)
    {
        var baseClient = new BetfairHttpClient(credentials.Certificate);
        var deserializer = new HttpDeserializer(baseClient);
        var tokenProvider = new TokenProvider(baseClient, credentials);
        var tokenInjector = new HttpTokenInjector(deserializer, tokenProvider, credentials.AppKey);
        return new HttpAdapter(tokenInjector);
    }
}