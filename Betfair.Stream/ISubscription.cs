namespace Betfair.Stream
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Stream.Responses;

    public interface ISubscription
    {
        void Connect();

        Task Authenticate();

        Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter);

        Task SubscribeToOrders();

        IAsyncEnumerable<ChangeMessage> GetChanges();

        void Disconnect();
    }
}