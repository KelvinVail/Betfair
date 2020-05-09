namespace Betfair.Betting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Xunit;

    public class LimitOrderTests
    {
        private readonly ExchangeServiceSpy service = new ExchangeServiceSpy();

        private readonly Orders orders;

        public LimitOrderTests()
        {
            this.orders = new Orders(this.service, "MarketId");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2147483648)]
        public void SelectionIdIsSetInConstructor(long selectionId)
        {
            var sut = new LimitOrder(selectionId, Side.Back, 1, 1.01);
            Assert.Equal(selectionId, sut.SelectionId);
        }

        [Theory]
        [InlineData(Side.Back)]
        [InlineData(Side.Lay)]
        public void SideCanBeSetInConstructor(Side side)
        {
            var sut = new LimitOrder(1, side, 1, 1.01);
            Assert.Equal(side, sut.Side);
        }

        [Theory]
        [InlineData(1.99)]
        [InlineData(10.77)]
        [InlineData(3.3333333)]
        public void SizeCanBeSetInConstructor(double size)
        {
            var sut = new LimitOrder(1, Side.Back, size, 1.01);
            Assert.Equal(size, sut.Size);
        }

        [Theory]
        [InlineData(1.01)]
        [InlineData(1000)]
        [InlineData(3.4567)]
        public void PriceCanBeSetInConstructor(double price)
        {
            var sut = new LimitOrder(1, Side.Back, 1, price);
            Assert.Equal(price, sut.Price);
        }

        [Theory]
        [InlineData(12345, Side.Back, 2.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 2.99, 1.01)]
        public void ToInstructionReturnCorrectJsonString(long selectionId, Side side, double size, double price)
        {
            var sut = new LimitOrder(selectionId, side, size, price);
            var expected = $"{{\"selectionId\":\"{selectionId}\"," +
                           $"\"side\":\"{side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"," +
                           "\"orderType\":\"LIMIT\"," +
                           "\"limitOrder\":{" +
                           $"\"size\":\"{size}\"," +
                           $"\"price\":\"{price}\"," +
                           "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(expected, sut.ToInstruction());
        }

        [Theory]
        [InlineData(2.999, 3)]
        [InlineData(3.3333333, 3.33)]
        [InlineData(2.555, 2.56)]
        [InlineData(10.6666, 10.67)]
        public void ToInstructionRoundsSizeToTwoDecimalsPlaces(double size, double rounded)
        {
            var sut = new LimitOrder(1, Side.Back, size, 1.01);
            var expected = "{\"selectionId\":\"1\"," +
                           "\"side\":\"BACK\"," +
                           "\"orderType\":\"LIMIT\"," +
                           "\"limitOrder\":{" +
                           $"\"size\":\"{rounded}\"," +
                           "\"price\":\"1.01\"," +
                           "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(expected, sut.ToInstruction());
        }

        [Theory]
        [InlineData(1, 1.01)]
        [InlineData(0.01, 1.01)]
        [InlineData(1.009, 1.01)]
        [InlineData(1.015, 1.01)]
        [InlineData(1.016, 1.02)]
        [InlineData(2.01, 2)]
        [InlineData(2.011, 2.02)]
        [InlineData(3.025, 3)]
        [InlineData(3.026, 3.05)]
        [InlineData(4.05, 4)]
        [InlineData(4.06, 4.1)]
        [InlineData(6.1, 6)]
        [InlineData(6.11, 6.2)]
        [InlineData(10.25, 10)]
        [InlineData(10.26, 10.5)]
        [InlineData(20.5, 20)]
        [InlineData(20.6, 21)]
        [InlineData(31, 30)]
        [InlineData(31.1, 32)]
        [InlineData(52.5, 50)]
        [InlineData(52.6, 55)]
        [InlineData(105, 100)]
        [InlineData(105.1, 110)]
        [InlineData(1001, 1000)]
        [InlineData(1000.1, 1000)]
        [InlineData(1000.00001, 1000)]
        [InlineData(9999, 1000)]
        public void PriceIsRoundedToNearestValidPrice(double price, double expected)
        {
            var sut = new LimitOrder(1, Side.Back, 2, price);
            var instruction = "{\"selectionId\":\"1\"," +
                              "\"side\":\"BACK\"," +
                              "\"orderType\":\"LIMIT\"," +
                              "\"limitOrder\":{" +
                              "\"size\":\"2\"," +
                              $"\"price\":\"{expected}\"," +
                              "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(instruction, sut.ToInstruction());
        }

        [Theory]
        [InlineData(2.00, 1.01, "SUCCESS")]
        [InlineData(9.99, 1000, "FAILURE")]
        public async Task ResultsAreSet(double size, double price, string status)
        {
            var sut = new LimitOrder(12345, Side.Back, size, price);
            var limitOrders = new List<LimitOrder>
            {
                new LimitOrder(98765, Side.Lay, -1, -1),
                sut,
            };
            await this.SetResults(limitOrders, status);
            Assert.NotEqual("1", sut.BetId);
            Assert.Equal("2", sut.BetId);
            Assert.Equal(size, sut.SizeMatched);
            Assert.Equal(price, sut.AveragePriceMatched);
            Assert.Equal(status, sut.Status);
            Assert.Equal("EXECUTION_COMPLETE", sut.OrderStatus);
            Assert.Equal(DateTime.Parse("2013-10-30T14:22:47.000Z", new DateTimeFormatInfo()), sut.PlacedDate);
        }

        [Fact]
        public async Task SetResultsShouldHandleMissingReport()
        {
            var sut = new LimitOrder(12345, Side.Back, 2.00, 1.01);
            this.orders.Add(sut);
            var limitOrders = new List<LimitOrder> { new LimitOrder(98765, Side.Lay, 2.00, 1.01) };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal(0, sut.SizeMatched);
        }

        [Theory]
        [InlineData(2.00)]
        [InlineData(9.99)]
        public async Task SetResultsShouldUseCorrectResult(double size)
        {
            var sut = new LimitOrder(12345, Side.Back, size, 1.01);
            var limitOrders = new List<LimitOrder>
            {
                new LimitOrder(98765, Side.Lay, -1, -1),
                new LimitOrder(12345, Side.Lay, -1, -1),
                sut,
            };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal(size, sut.SizeMatched);
        }

        [Theory]
        [InlineData(12345, Side.Back, 1.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 1.99, 1.01)]
        public async Task ToCancelInstructionReturnCorrectJsonString(long selectionId, Side side, double size, double price)
        {
            var sut = new LimitOrder(selectionId, side, size, price);
            await this.SetResults(new List<LimitOrder> { sut }, "SUCCESS", "EXECUTABLE");
            var expected = $"{{\"betId\":\"{sut.BetId}\"}}";
            Assert.Equal(expected, sut.ToCancelInstruction());
        }

        [Theory]
        [InlineData(12345, Side.Back, 1.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 1.99, 1.01)]
        public async Task ToCancelInstructionShouldReturnNullIfOrderIsComplete(long selectionId, Side side, double size, double price)
        {
            var sut = new LimitOrder(selectionId, side, size, price);
            await this.SetResults(new List<LimitOrder> { sut }, "SUCCESS");
            Assert.Null(sut.ToCancelInstruction());
        }

        [Theory]
        [InlineData(1.99, 1.01, 2, 1000)]
        [InlineData(2, 1.01, 2, 1.01)]
        [InlineData(1.50, 2.2, 2, 1000)]
        [InlineData(2, 2.2, 2, 2.2)]
        [InlineData(1.99, 5, 2, 1000)]
        [InlineData(2, 5, 2, 5)]
        [InlineData(1.97, 5.1, 1.97, 5.1)]
        [InlineData(1.96, 5.1, 2, 1000)]
        [InlineData(1, 10, 1, 10)]
        [InlineData(0.99, 10, 2, 1000)]
        [InlineData(0.10, 100, 0.10, 100)]
        [InlineData(0.09, 100, 2, 1000)]
        [InlineData(0.01, 1000, 0.01, 1000)]
        public void HandleBelowMinimumStakeForBackOrders(double size, double price, double expectedSize, double expectedPrice)
        {
            var sut = new LimitOrder(12345, Side.Back, size, price);
            var instruction = "{\"selectionId\":\"12345\"," +
                              "\"side\":\"BACK\"," +
                              "\"orderType\":\"LIMIT\"," +
                              "\"limitOrder\":{" +
                              $"\"size\":\"{expectedSize}\"," +
                              $"\"price\":\"{expectedPrice}\"," +
                              "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(instruction, sut.ToInstruction());
        }

        [Theory]
        [InlineData(1.99, 1.01, 2, 1.01)]
        [InlineData(2, 1.01, 2, 1.01)]
        [InlineData(1.50, 2.2, 2, 1.01)]
        [InlineData(2, 2.2, 2, 2.2)]
        [InlineData(1.99, 5, 2, 1.01)]
        [InlineData(2, 5, 2, 5)]
        [InlineData(1.97, 5.1, 1.97, 5.1)]
        [InlineData(1.96, 5.1, 2, 1.01)]
        [InlineData(1, 10, 1, 10)]
        [InlineData(0.99, 10, 2, 1.01)]
        [InlineData(0.10, 100, 0.10, 100)]
        [InlineData(0.09, 100, 2, 1.01)]
        [InlineData(0.01, 1000, 0.01, 1000)]
        public void HandleBelowMinimumStakeForLayOrders(double size, double price, double expectedSize, double expectedPrice)
        {
            var sut = new LimitOrder(12345, Side.Lay, size, price);
            var instruction = "{\"selectionId\":\"12345\"," +
                              "\"side\":\"LAY\"," +
                              "\"orderType\":\"LIMIT\"," +
                              "\"limitOrder\":{" +
                              $"\"size\":\"{expectedSize}\"," +
                              $"\"price\":\"{expectedPrice}\"," +
                              "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(instruction, sut.ToInstruction());
        }

        [Theory]
        [InlineData(1.99, 2, true)]
        [InlineData(1.99, 1.01, true)]
        [InlineData(2, 1.01, false)]
        [InlineData(1.50, 2.2, true)]
        [InlineData(2, 2.2, false)]
        [InlineData(1.99, 5, true)]
        [InlineData(2, 5, false)]
        [InlineData(1.97, 5.1, false)]
        [InlineData(1.96, 5.1, true)]
        [InlineData(1, 10, false)]
        [InlineData(0.99, 10, true)]
        [InlineData(0.10, 100, false)]
        [InlineData(0.09, 100, true)]
        [InlineData(0.01, 1000, false)]
        public void BelowMinimumStakeFlagIsSet(double size, double price, bool expected)
        {
            var sut = new LimitOrder(12345, Side.Lay, size, price);
            Assert.Equal(expected, sut.BelowMinimumStake);
        }

        [Theory]
        [InlineData(1.99, 2, 0.01)]
        [InlineData(1.90, 2, 0.10)]
        [InlineData(1.75, 2, 0.25)]
        [InlineData(1.50, 2, 0.50)]
        [InlineData(1, 2, 1)]
        [InlineData(0.5, 2, 1.5)]
        [InlineData(0.01, 2, 1.99)]
        [InlineData(0.01, 6, 1.99)]
        [InlineData(0.01, 7.2, 1.99)]
        public void IfBelowMinimumStakeThenToBelowMinimumCancelInstructionShouldBeSet(double size, double price, double reduction)
        {
            var sut = new LimitOrder(12345, Side.Lay, size, price);
            var expected = $"{{\"betId\":\"{sut.BetId}\",\"sizeReduction\":{reduction}}}";
            Assert.Equal(expected, sut.ToBelowMinimumCancelInstruction());
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 5)]
        [InlineData(1.43, 7)]
        [InlineData(1, 10)]
        [InlineData(0.1, 100)]
        public void IfAboveMinimumStakeToBelowMinimumInstructionsShouldBeNull(double size, double price)
        {
            var sut = new LimitOrder(12345, Side.Lay, size, price);
            Assert.Null(sut.ToBelowMinimumCancelInstruction());
            Assert.Null(sut.ToBelowMinimumReplaceInstruction());
        }

        [Theory]
        [InlineData(1.99, 2)]
        [InlineData(1.90, 2)]
        [InlineData(1.75, 2)]
        [InlineData(1.50, 2)]
        [InlineData(1, 2)]
        [InlineData(0.5, 2)]
        [InlineData(0.01, 2)]
        public void IfBelowMinimumStakeThenToBelowMinimumReplaceInstructionShouldBeSet(double size, double price)
        {
            var sut = new LimitOrder(12345, Side.Lay, size, price);
            var expected = $"{{\"betId\":\"{sut.BetId}\",\"newPrice\":{price}}}";
            Assert.Equal(expected, sut.ToBelowMinimumReplaceInstruction());
        }

        private async Task SetResults(List<LimitOrder> limitOrders, string status, string orderStatus = "EXECUTION_COMPLETE")
        {
            var instructions = string.Empty;
            var i = 0;
            foreach (var limitOrder in limitOrders)
            {
                i++;
                instructions += GetResult(limitOrder, i, status, orderStatus) + ",";
            }

            instructions = instructions.Remove(instructions.Length - 1, 1);

            this.service.WithReturnContent(
                "placeOrders",
                $"{{\"marketId\":\"MarketId\",\"instructionReports\":[{instructions}], \"status\":\"SUCCESS\"}}");

            limitOrders.ForEach(o => this.orders.Add(o));
            await this.orders.PlaceAsync();
        }

        private static string GetResult(LimitOrder limitOrder, long betId, string status, string orderStatus)
        {
            return "{" +
                   "\"instruction\":" +
                   "{" +
                   $"\"selectionId\":{limitOrder.SelectionId}," +
                   "\"limitOrder\":{" +
                   $"\"size\":{limitOrder.Size}," +
                   $"\"price\":{limitOrder.Price}," +
                   "\"persistenceType\":\"LAPSE\"},\"orderType\":\"LIMIT\"," +
                   $"\"side\":\"{limitOrder.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"" +
                   "}" +
                   "," +
                   $"\"betId\":\"{betId}\"," +
                   "\"placedDate\":\"2013-10-30T14:22:47.000Z\"," +
                   $"\"averagePriceMatched\":{limitOrder.Price}," +
                   $"\"sizeMatched\":{limitOrder.Size}," +
                   $"\"orderStatus\":\"{orderStatus}\"," +
                   $"\"status\":\"{status}\"" +
                   "}";
        }

        private static string GetReplaceReport(LimitOrder limitOrder, string status, string newBetId)
        {
            return "{\"jsonrpc\":\"2.0\"," +
                   "\"result\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"marketId\":\"MarketId\"," +
                   "\"instructionReports\":" +
                   $"[{{\"status\":\"{status}\"," +
                   "\"cancelInstructionReport\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"instruction\":" +
                   $"{{\"betId\":\"{limitOrder.BetId}\"}}," +
                   $"\"sizeCancelled\":{limitOrder.Size}," +
                   "\"cancelledDate\":\"2020-05-09T19:53:50.000Z\"}," +
                   "\"placeInstructionReport\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"instruction\":" +
                   $"{{\"selectionId\":{limitOrder.SelectionId}," +
                   "\"limitOrder\":" +
                   $"{{\"size\":{limitOrder.Size}," +
                   $"\"price\":{limitOrder.Price}," +
                   "\"persistenceType\":\"LAPSE\"}," +
                   "\"orderType\":\"LIMIT\"," +
                   $"\"side\":\"{limitOrder.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"}}," +
                   $"\"betId\":\"{newBetId}\"," +
                   "\"placedDate\":\"2020-05-09T19:53:50.000Z\"," +
                   "\"averagePriceMatched\":0.0," +
                   "\"sizeMatched\":0.0," +
                   "\"orderStatus\":\"EXECUTABLE\"}}]}," +
                   "\"id\":1}";
        }
    }
}
