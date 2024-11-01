using Betfair.Core.Login;
using Betfair.Extensions.Contracts;
using Betfair.Stream;

namespace Betfair.Extensions;

internal class MarketSubscription : Subscription, ISubscription
{
    public MarketSubscription(Credentials credentials)
        : base(credentials)
    {
    }
}