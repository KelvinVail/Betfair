namespace Betfair.Betting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Xunit;

    public class LimitOrderTests
    {
        private readonly ExchangeServiceSpy service = new ExchangeServiceSpy();

        private readonly OrderService orderService;

        public LimitOrderTests()
        {
            this.orderService = new OrderService(this.service);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2147483648)]
        public void SelectionIdIsSetInConstructor(long selectionId)
        {
            var sut = new LimitOrder(selectionId, Side.Back, 1.01, 1);
            Assert.Equal(selectionId, sut.SelectionId);
        }

        [Theory]
        [InlineData(Side.Back)]
        [InlineData(Side.Lay)]
        public void SideCanBeSetInConstructor(Side side)
        {
            var sut = new LimitOrder(1, side, 1.01, 1);
            Assert.Equal(side, sut.Side);
        }

        [Theory]
        [InlineData(1.99)]
        [InlineData(10.77)]
        [InlineData(3.3333333)]
        public void SizeCanBeSetInConstructor(double size)
        {
            var sut = new LimitOrder(1, Side.Back, 1.01, size);
            Assert.Equal(size, sut.Size);
        }

        [Theory]
        [InlineData(1.01)]
        [InlineData(1000)]
        [InlineData(3.4567)]
        public void PriceCanBeSetInConstructor(double price)
        {
            var sut = new LimitOrder(1, Side.Back, price, 1);
            Assert.Equal(price, sut.Price);
        }

        [Theory]
        [InlineData(12345, Side.Back, 2.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 2.99, 1.01)]
        public void ToInstructionReturnCorrectJsonString(long selectionId, Side side, double size, double price)
        {
            var sut = new LimitOrder(selectionId, side, price, size);
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
            var sut = new LimitOrder(1, Side.Back, 1.01, size);
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
            var sut = new LimitOrder(1, Side.Back, price, 2);
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
            var sut = new LimitOrder(12345, Side.Back, price, size);
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
            var sut = new LimitOrder(12345, Side.Back, 1.01, 2.00);
            var limitOrders = new List<LimitOrder> { new LimitOrder(98765, Side.Lay, 1.01, 2.00) };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal(0, sut.SizeMatched);
        }

        [Fact]
        public async Task SetResultsShouldHandleNullReport()
        {
            var limitOrders = new List<LimitOrder> { new LimitOrder(98765, Side.Lay, 1.01, 2.00) };
            await this.orderService.Place("MarketId", limitOrders);
        }

        [Theory]
        [InlineData(2.00)]
        [InlineData(9.99)]
        public async Task SetResultsShouldUseCorrectResult(double size)
        {
            var sut = new LimitOrder(12345, Side.Back, 1.01, size);
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
            var sut = new LimitOrder(selectionId, side, price, size);
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
            var sut = new LimitOrder(selectionId, side, price, size);
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
            var sut = new LimitOrder(12345, Side.Back, price, size);
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
        [InlineData(0.10, 100, 0.10, 100)]
        [InlineData(0.01, 1000, 0.01, 1000)]
        public void HandleBelowMinimumStakeForLayOrders(double size, double price, double expectedSize, double expectedPrice)
        {
            var sut = new LimitOrder(12345, Side.Lay, price, size);
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
        [InlineData(0.01, 2.2, 2)]
        [InlineData(0.02, 1.8, 1.5)]
        [InlineData(0.03, 1.4, 1.34)]
        [InlineData(0.11, 1.12, 1.1)]
        [InlineData(0.7, 1.05, 1.02)]
        [InlineData(1, 1.05, 1.01)]
        [InlineData(1.01, 1.05, 1.01)]
        public void InitialPriceUsedForVerySmallLaysShouldReturnAtLeastOnePenceProfit(double size, double price, double initialPrice)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            var instruction = "{\"selectionId\":\"12345\"," +
                              "\"side\":\"LAY\"," +
                              "\"orderType\":\"LIMIT\"," +
                              "\"limitOrder\":{" +
                              $"\"size\":\"2\"," +
                              $"\"price\":\"{initialPrice}\"," +
                              "\"persistenceType\":\"LAPSE\"}}";
            Assert.Equal(instruction, order.ToInstruction());
        }

        [Theory]
        [InlineData(1.1, 0.05, 0.1)]
        [InlineData(1.06, 0.08, 0.17)]
        [InlineData(1.05, 0.1, 0.2)]
        [InlineData(1.5, 0.01, 0.02)]
        [InlineData(1.01, 0.01, 1)]
        public void LayOrderPlacedAtBelowMinimumShouldBeAdjustedToReturnAtLeastOnePenceProfit(double price, double size, double expectedSize)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            Assert.Equal(expectedSize, order.Size);

            var backOrder = new LimitOrder(12345, Side.Back, price, size);
            Assert.Equal(size, backOrder.Size);
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
            var sut = new LimitOrder(12345, Side.Lay, price, size);
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
            var sut = new LimitOrder(12345, Side.Lay, price, size);
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
            var sut = new LimitOrder(12345, Side.Lay, price, size);
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
            var sut = new LimitOrder(12345, Side.Lay, price, size);
            var expected = $"{{\"betId\":\"{sut.BetId}\",\"newPrice\":{price}}}";
            Assert.Equal(expected, sut.ToBelowMinimumReplaceInstruction());
        }

        [Fact]
        public async Task BetIdIsUpdatedWhenOrderIsReplaced()
        {
            var sut = new LimitOrder(12345, Side.Lay, 2, 0.5);
            await this.SetResults(new List<LimitOrder> { sut }, "SUCCESS");
            Assert.Equal("2", sut.BetId);
        }

        [Fact]
        public async Task SetResultsShouldUpdateCorrectBetId()
        {
            var order1 = new LimitOrder(12345, Side.Back, 1.01, 0.5); // betId = 1, newId = 4
            var order2 = new LimitOrder(98765, Side.Lay, 2, 0.5); // betId = 2, newId = 5
            var order3 = new LimitOrder(12345, Side.Lay, 3, 0.5); // betId = 3, newId = 6
            var limitOrders = new List<LimitOrder>
            {
                order1,
                order2,
                order3,
            };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal("4", order1.BetId);
            Assert.Equal("5", order2.BetId);
            Assert.Equal("6", order3.BetId);
        }

        [Theory]
        [InlineData(12345, Side.Back, 2, 1.01)]
        [InlineData(11111, Side.Lay, 2, 2)]
        [InlineData(98765, Side.Back, 2, 3.5)]
        public async Task CancelSendsCancelInstruction(long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var limitOrder2 = new LimitOrder(1, Side.Back, 2, 10);
            var limitOrders = new List<LimitOrder> { limitOrder, limitOrder2 };
            await this.SetResults(limitOrders, "SUCCESS", "EXECUTABLE");
            var cancelInstruction = $"{{\"marketId\":\"MarketId\",\"instructions\":[{limitOrder.ToCancelInstruction()},{limitOrder2.ToCancelInstruction()}]}}";
            await this.orderService.Cancel("MarketId", limitOrders.Select(o => o.BetId).ToList());
            Assert.Equal(cancelInstruction, this.service.SentParameters["cancelOrders"]);
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

        private static string GetReplaceReport(LimitOrder limitOrder, string status, long newBetId, long originalBetId, string orderStatus)
        {
            return $"{{\"status\":\"{status}\"," +
                   "\"cancelInstructionReport\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"instruction\":" +
                   $"{{\"betId\":\"{originalBetId}\"}}," +
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
                   $"\"orderStatus\":\"{orderStatus}\"}}}}";
        }

        private async Task SetResults(List<LimitOrder> limitOrders, string status, string orderStatus = "EXECUTION_COMPLETE")
        {
            var instructions = string.Empty;
            var betId = 0;
            foreach (var limitOrder in limitOrders)
            {
                betId++;
                instructions += GetResult(limitOrder, betId, status, orderStatus) + ",";
            }

            instructions = instructions.Remove(instructions.Length - 1, 1);

            this.SetPlaceReturnContent(instructions);

            var replaceInstructions = string.Empty;
            var originalBetId = 0;
            foreach (var limitOrder in limitOrders.Where(o => o.BelowMinimumStake))
            {
                betId++;
                originalBetId++;
                replaceInstructions += GetReplaceReport(limitOrder, orderStatus, betId, originalBetId, orderStatus) + ",";
            }

            if (limitOrders.Any(o => o.BelowMinimumStake))
            {
                replaceInstructions = replaceInstructions.Remove(replaceInstructions.Length - 1, 1);

                this.SetReplaceReturnContent(replaceInstructions);
            }

            await this.orderService.Place("MarketId", limitOrders);
        }

        private void SetPlaceReturnContent(string instructions)
        {
            this.service.WithReturnContent(
                "placeOrders",
                $"{{\"marketId\":\"MarketId\",\"instructionReports\":[{instructions}], \"status\":\"SUCCESS\"}}");
        }

        private void SetReplaceReturnContent(string replaceInstructions)
        {
            this.service.WithReturnContent(
                "replaceOrders",
                $"{{\"marketId\":\"MarketId\",\"instructionReports\":[{replaceInstructions}], \"status\":\"SUCCESS\"}}");
        }
    }
}
