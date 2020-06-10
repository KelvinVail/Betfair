namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class OrderChangeStub : OrderChange
    {
        public OrderChangeStub()
        {
            this.MarketId = "1.2345";
        }

        public OrderChangeStub WithOrderRunnerChange(OrderRunnerChange orc)
        {
            this.OrderRunnerChanges ??= new List<OrderRunnerChange>();
            this.OrderRunnerChanges.Add(orc);
            return this;
        }
    }
}
