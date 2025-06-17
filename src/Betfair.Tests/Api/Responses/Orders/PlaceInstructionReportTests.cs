using Betfair.Api.Betting.Endpoints.PlaceOrders.Requests;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;
using Betfair.Api.Betting.Enums;

namespace Betfair.Tests.Api.Responses.Orders;

public class PlaceInstructionReportTests
{
    [Fact]
    public void StatusPropertyShouldBeEmptyByDefault()
    {
        var report = new PlaceInstructionReport();
        report.Status.Should().Be(string.Empty);
    }

    [Fact]
    public void ErrorCodePropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void BetIdPropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.BetId.Should().BeNull();
    }

    [Fact]
    public void PlacedDatePropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.PlacedDate.Should().BeNull();
    }

    [Fact]
    public void AveragePriceMatchedPropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.AveragePriceMatched.Should().BeNull();
    }

    [Fact]
    public void SizeMatchedPropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.SizeMatched.Should().BeNull();
    }

    [Fact]
    public void OrderStatusPropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.OrderStatus.Should().BeNull();
    }

    [Fact]
    public void InstructionPropertyShouldBeNullByDefault()
    {
        var report = new PlaceInstructionReport();
        report.Instruction.Should().BeNull();
    }

    [Fact]
    public void StatusPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { Status = "Test" };
        report.Status.Should().Be("Test");
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { ErrorCode = "Test" };
        report.ErrorCode.Should().Be("Test");
    }

    [Fact]
    public void BetIdPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { BetId = "Test" };
        report.BetId.Should().Be("Test");
    }

    [Fact]
    public void PlacedDatePropertyShouldAcceptNonNullValue()
    {
        var date = DateTimeOffset.Now;
        var report = new PlaceInstructionReport { PlacedDate = date };
        report.PlacedDate.Should().Be(date);
    }

    [Fact]
    public void AveragePriceMatchedPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { AveragePriceMatched = 1.23 };
        report.AveragePriceMatched.Should().Be(1.23);
    }

    [Fact]
    public void SizeMatchedPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { SizeMatched = 1.23 };
        report.SizeMatched.Should().Be(1.23);
    }

    [Fact]
    public void OrderStatusPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { OrderStatus = "Test" };
        report.OrderStatus.Should().Be("Test");
    }

    [Fact]
    public void InstructionPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceInstructionReport { Instruction = new PlaceInstruction() };
        report.Instruction.Should().NotBeNull();
    }

    [Fact]
    public void BetIdPropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { BetId = null };
        report.BetId.Should().BeNull();
    }

    [Fact]
    public void PlacedDatePropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { PlacedDate = null };
        report.PlacedDate.Should().BeNull();
    }

    [Fact]
    public void AveragePriceMatchedPropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { AveragePriceMatched = null };
        report.AveragePriceMatched.Should().BeNull();
    }

    [Fact]
    public void SizeMatchedPropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { SizeMatched = null };
        report.SizeMatched.Should().BeNull();
    }

    [Fact]
    public void OrderStatusPropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { OrderStatus = null };
        report.OrderStatus.Should().BeNull();
    }

    [Fact]
    public void InstructionPropertyShouldAcceptNull()
    {
        var report = new PlaceInstructionReport { Instruction = null };
        report.Instruction.Should().BeNull();
    }

    [Fact]
    public void ShouldBeDeserializable()
    {
        const string json = "{\"status\":\"SUCCESS\",\"errorCode\":\"INVALID_ACCOUNT_STATE\",\"betId\":\"123456789\",\"placedDate\":\"2021-10-01T00:00:00Z\",\"averagePriceMatched\":1.23,\"sizeMatched\":1.23,\"orderStatus\":\"EXECUTION_COMPLETE\",\"instruction\":{\"selectionId\":123456789,\"handicap\":1.23,\"side\":\"BACK\",\"orderType\":\"LIMIT\",\"limitOrder\":{\"size\":1.23,\"price\":1.23}}}";

        var report = JsonSerializer.Deserialize(json, SerializerContext.Default.PlaceInstructionReport);

        report.Should().NotBeNull();
        report!.Status.Should().Be("SUCCESS");
        report.ErrorCode.Should().Be("INVALID_ACCOUNT_STATE");
        report.BetId.Should().Be("123456789");
        report.PlacedDate.Should().Be(new DateTimeOffset(2021, 10, 1, 0, 0, 0, TimeSpan.Zero));
        report.AveragePriceMatched.Should().Be(1.23);
        report.SizeMatched.Should().Be(1.23);
        report.OrderStatus.Should().Be("EXECUTION_COMPLETE");
        report.Instruction.Should().NotBeNull();
        report.Instruction!.SelectionId.Should().Be(123456789);
        report.Instruction!.Handicap.Should().Be(1.23);
        report.Instruction!.Side.Should().Be(Side.Back);
        report.Instruction!.OrderType.Should().Be(OrderType.Limit);
        report.Instruction!.LimitOrder.Should().NotBeNull();
        report.Instruction!.LimitOrder!.Size.Should().Be(1.23);
        report.Instruction!.LimitOrder!.Price.Should().Be(1.23);
    }
}
