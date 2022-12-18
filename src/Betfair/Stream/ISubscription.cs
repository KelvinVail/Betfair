using System.Collections.Generic;
using System.Threading.Tasks;
using Betfair.Stream.Responses;

namespace Betfair.Stream
{
    public interface ISubscription
    {
        void Connect();

        Task Authenticate();

        Task Subscribe(StreamMarketFilter streamMarketFilter, MarketDataFilter dataFilter);

        Task SubscribeToOrders();

        IAsyncEnumerable<ChangeMessage> GetChanges();

        void Disconnect();
    }
}