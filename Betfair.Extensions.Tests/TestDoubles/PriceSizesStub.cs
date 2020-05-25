namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;

    public class PriceSizesStub : List<List<double>>
    {
        public PriceSizesStub WithPriceSize(double price, double size)
        {
            var priceSize = new List<double> { price, size };
            this.Add(priceSize);
            return this;
        }
    }
}
