namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class OrderRunnerChangeStub : OrderRunnerChange
    {
        public OrderRunnerChangeStub()
        {
            this.SelectionId = 12345;
        }

        public OrderRunnerChangeStub WithMatchedBack(double? price, double? size)
        {
            this.MatchedBacks ??= new List<List<double?>>();
            this.MatchedBacks.Add(new List<double?> { price, size });
            return this;
        }

        public OrderRunnerChangeStub WithMatchedLay(double? price, double? size)
        {
            this.MatchedLays ??= new List<List<double?>>();
            this.MatchedLays.Add(new List<double?> { price, size });
            return this;
        }
    }
}
