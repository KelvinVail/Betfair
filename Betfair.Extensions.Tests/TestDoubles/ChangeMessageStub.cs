namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class ChangeMessageStub
    {
        private ChangeMessage change = new ChangeMessage();

        public ChangeMessage Build()
        {
            return this.change;
        }

        public ChangeMessageStub New()
        {
            this.change = new ChangeMessage();
            return this;
        }

        public ChangeMessageStub WithMarketChange(MarketChange mc)
        {
            this.change.MarketChanges ??= new List<MarketChange>();
            this.change.OrderChanges = null;
            this.change.Operation = "mcm";
            this.change.MarketChanges.Add(mc);
            return this;
        }

        public ChangeMessageStub WithOrderChange(OrderChange oc)
        {
            this.change.OrderChanges ??= new List<OrderChange>();
            this.change.MarketChanges = null;
            this.change.Operation = "ocm";
            this.change.OrderChanges.Add(oc);
            return this;
        }

        public ChangeMessageStub WithPublishTime(long? pt)
        {
            this.change.PublishTime = pt;
            return this;
        }
    }
}
