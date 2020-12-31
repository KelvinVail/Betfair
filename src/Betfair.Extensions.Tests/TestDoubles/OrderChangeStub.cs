using System.Collections.Generic;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class OrderChangeStub : OrderChange
    {
        public OrderChangeStub(string marketId = "1.2345")
        {
            MarketId = marketId;
        }

        public OrderChangeStub WithOrderRunnerChange(OrderRunnerChange orc)
        {
            OrderRunnerChanges ??= new List<OrderRunnerChange>();
            OrderRunnerChanges.Add(orc);
            return this;
        }
    }
}
