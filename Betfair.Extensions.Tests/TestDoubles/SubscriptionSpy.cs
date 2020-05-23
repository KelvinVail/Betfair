namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class SubscriptionSpy : ISubscription
    {
        public string Actions { get; private set; }

        public IList<ChangeMessage> Messages { get; } = new List<ChangeMessage> { new ChangeMessage { Clock = "a" } };

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
            foreach (var changeMessage in this.Messages)
            {
                this.Actions += "M";
                var result = await Task.FromResult(changeMessage);
                yield return result;
            }
        }

        public void Disconnect()
        {
            this.Actions += "D";
        }
    }
}
