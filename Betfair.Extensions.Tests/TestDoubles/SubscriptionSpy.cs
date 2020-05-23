namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class SubscriptionSpy : ISubscription
    {
        public string Actions { get; private set; }

        public void Connect()
        {
            this.Actions += "C";
        }

        public async Task Authenticate()
        {
            this.Actions += "A";
            await Task.CompletedTask;
        }

        public async Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter)
        {
            this.Actions += "S";
            await Task.CompletedTask;
        }

        public async Task SubscribeToOrders()
        {
            this.Actions += "O";
            await Task.CompletedTask;
        }

        public async IAsyncEnumerable<ChangeMessage> GetChanges()
        {
            for (var i = 0; i < 1; i++)
            {
                this.Actions += "M";
                var result = await Task.FromResult(new ChangeMessage());
                yield return result;
            }
        }

        public void Disconnect()
        {
            this.Actions += "D";
        }
    }
}
