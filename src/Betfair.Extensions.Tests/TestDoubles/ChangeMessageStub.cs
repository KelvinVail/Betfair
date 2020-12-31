using System.Collections.Generic;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class ChangeMessageStub
    {
        private ChangeMessage _change = new ChangeMessage();

        public ChangeMessage Build()
        {
            return _change;
        }

        public ChangeMessageStub New()
        {
            _change = new ChangeMessage();
            return this;
        }

        public ChangeMessageStub WithMarketChange(MarketChange mc)
        {
            _change.MarketChanges ??= new List<MarketChange>();
            _change.OrderChanges = null;
            _change.Operation = "mcm";
            _change.MarketChanges.Add(mc);
            return this;
        }

        public ChangeMessageStub WithOrderChange(OrderChange oc)
        {
            _change.OrderChanges ??= new List<OrderChange>();
            _change.MarketChanges = null;
            _change.Operation = "ocm";
            _change.OrderChanges.Add(oc);
            return this;
        }

        public ChangeMessageStub WithPublishTime(long? pt)
        {
            _change.PublishTime = pt;
            return this;
        }
    }
}
