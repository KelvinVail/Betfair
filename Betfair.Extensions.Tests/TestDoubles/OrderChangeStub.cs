namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class OrderChangeStub : OrderChange
    {
        public OrderChangeStub(string marketId = "1.2345")
        {
            this.MarketId = marketId;
        }

        public OrderChangeStub WithOrderRunnerChange(OrderRunnerChange orc)
        {
            this.OrderRunnerChanges ??= new List<OrderRunnerChange>();
            this.OrderRunnerChanges.Add(orc);
            return this;
        }
    }
}
