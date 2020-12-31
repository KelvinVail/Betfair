using System.Collections.Generic;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class PriceSizesStub : List<List<double?>>
    {
        public PriceSizesStub WithPriceSize(double? price, double? size)
        {
            var priceSize = new List<double?> { price, size };
            Add(priceSize);
            return this;
        }
    }
}
