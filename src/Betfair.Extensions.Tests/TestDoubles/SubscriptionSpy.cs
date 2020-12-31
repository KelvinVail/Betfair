using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class SubscriptionSpy : ISubscription
    {
        private int _messagesToProcess;
        private CancellationTokenSource _tokenSource;

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
                PublishTime = PublishTime,
            };
            Messages.Add(change);
            return this;
        }

        public SubscriptionSpy WithMarketChanges(IEnumerable<MarketChange> marketChanges)
        {
            var change = new ChangeMessage
            {
                Operation = "mcm",
                ChangeType = "mc",
                MarketChanges = marketChanges.ToList(),
                PublishTime = PublishTime,
            };
            Messages.Add(change);
            return this;
        }

        public SubscriptionSpy WithOrderChange(OrderChange orderChange)
        {
            var change = new ChangeMessage
            {
                Operation = "orc",
                ChangeType = "oc",
                OrderChanges = new List<OrderChange> { orderChange },
                PublishTime = PublishTime,
            };
            Messages.Add(change);
            return this;
        }

        public void CancelAfterThisManyMessages(int messages, CancellationTokenSource source)
        {
            _messagesToProcess = messages;
            _tokenSource = source;
        }

        public void Connect()
        {
            Actions += "C";
        }

        public async Task Authenticate()
        {
            Actions += "A";
            await Task.CompletedTask;
        }

        public async Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter)
        {
            Actions += "S";
            MarketId = marketFilter?.MarketIds?.SingleOrDefault();
            Fields = dataFilter?.Fields;
            LadderLevels = dataFilter?.LadderLevels;
            await Task.CompletedTask;
        }

        public async Task SubscribeToOrders()
        {
            Actions += "O";
            await Task.CompletedTask;
        }

        public async IAsyncEnumerable<ChangeMessage> GetChanges()
        {
            foreach (var changeMessage in Messages)
            {
                Actions += "M";
                _messagesToProcess -= 1;
                if (_messagesToProcess <= 0) _tokenSource?.Cancel();
                var result = await Task.FromResult(changeMessage);
                yield return result;
            }
        }

        public void Disconnect()
        {
            Actions += "D";
        }
    }
}
