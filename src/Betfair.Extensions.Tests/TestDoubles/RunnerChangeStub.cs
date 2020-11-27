namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class RunnerChangeStub : RunnerChange
    {
        public RunnerChangeStub()
        {
            this.SelectionId = 12345;
        }

        public RunnerChangeStub WithSelectionId(long? selectionId)
        {
            this.SelectionId = selectionId;
            return this;
        }

        public RunnerChangeStub WithLastTradedPrice(double lastTradedPrice)
        {
            this.LastTradedPrice = lastTradedPrice;
            return this;
        }

        public RunnerChangeStub WithTotalMatched(double totalMatched)
        {
            this.TotalMatched = totalMatched;
            return this;
        }

        public RunnerChangeStub WithBestAvailableToBack(double? level, double? price, double? size)
        {
            this.BestAvailableToBack ??= new List<List<double?>>();
            var levelPriceSize = new List<double?> { level, price, size };
            this.BestAvailableToBack.Add(levelPriceSize);
            return this;
        }

        public RunnerChangeStub WithBestAvailableToLay(double? level, double? price, double? size)
        {
            this.BestAvailableToLay ??= new List<List<double?>>();
            var levelPriceSize = new List<double?> { level, price, size };
            this.BestAvailableToLay.Add(levelPriceSize);
            return this;
        }

        public RunnerChangeStub WithTraded(double? price, double? size)
        {
            this.Traded ??= new List<List<double?>>();
            var priceSize = new List<double?> { price, size };
            this.Traded.Add(priceSize);
            return this;
        }
    }
}
