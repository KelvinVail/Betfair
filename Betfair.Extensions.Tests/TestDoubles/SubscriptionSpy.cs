namespace Betfair.Extensions.Tests.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class SubscriptionSpy : ISubscription
    {
        private int messagesToProcess;

        private CancellationTokenSource tokenSource;

        public string Actions { get; private set; }

        public string MarketId { get; private set; }

        public HashSet<string> Fields { get; private set; }

        public int? LadderLevels { get; private set; }

        public IList<ChangeMessage> Messages { get; } = new List<ChangeMessage> { new ChangeMessage { Clock = "a" } };

        public long PublishTime { get; set; }

        public SubscriptionSpy WithMarketChange(MarketChange marketChange)
        {
            var change = new ChangeMessage
            {
                Operation = "mcm",
                ChangeType = "mc",
                MarketChanges = new List<MarketChange> { marketChange },
                PublishTime = this.PublishTime,
            };
            this.Messages.Add(change);
            return this;
        }

        public SubscriptionSpy WithMarketChanges(IEnumerable<MarketChange> marketChanges)
        {
            var change = new ChangeMessage
            {
                Operation = "mcm",
                ChangeType = "mc",
                MarketChanges = marketChanges.ToList(),
                PublishTime = this.PublishTime,
            };
            this.Messages.Add(change);
            return this;
        }

        public void CancelAfterThisManyMessages(int messages, CancellationTokenSource source)
        {
            this.messagesToProcess = messages;
            this.tokenSource = source;
        }

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
            this.MarketId = marketFilter?.MarketIds?.SingleOrDefault();
            this.Fields = dataFilter?.Fields;
            this.LadderLevels = dataFilter?.LadderLevels;
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
                this.messagesToProcess -= 1;
                if (this.messagesToProcess <= 0) this.tokenSource?.Cancel();
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
