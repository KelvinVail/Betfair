using System.Collections.Generic;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class RunnerChangeStub : RunnerChange
    {
        public RunnerChangeStub()
        {
            SelectionId = 12345;
        }

        public RunnerChangeStub WithSelectionId(long? selectionId)
        {
            SelectionId = selectionId;
            return this;
        }

        public RunnerChangeStub WithLastTradedPrice(double lastTradedPrice)
        {
            LastTradedPrice = lastTradedPrice;
            return this;
        }

        public RunnerChangeStub WithTotalMatched(double totalMatched)
        {
            TotalMatched = totalMatched;
            return this;
        }

        public RunnerChangeStub WithBestAvailableToBack(double? level, double? price, double? size)
        {
            BestAvailableToBack ??= new List<List<double?>>();
            var levelPriceSize = new List<double?> { level, price, size };
            BestAvailableToBack.Add(levelPriceSize);
            return this;
        }

        public RunnerChangeStub WithBestAvailableToLay(double? level, double? price, double? size)
        {
            BestAvailableToLay ??= new List<List<double?>>();
            var levelPriceSize = new List<double?> { level, price, size };
            BestAvailableToLay.Add(levelPriceSize);
            return this;
        }

        public RunnerChangeStub WithTraded(double? price, double? size)
        {
            Traded ??= new List<List<double?>>();
            var priceSize = new List<double?> { price, size };
            Traded.Add(priceSize);
            return this;
        }
    }
}
