using Betfair.Core.Enums;
using Betfair.Stream.OrderCache;

namespace Betfair.Tests.Stream.OrderCache;

public class UnmatchedOrderCacheTests
{
    [Fact]
    public void BetIdIsSetFromConstructor()
    {
        var order = new UnmatchedOrderCache("bet123"u8.ToArray());

        order.BetId.Should().Be("bet123");
    }

    [Fact]
    public void DoubleFieldsDefaultToNaN()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray());

        order.Price.Should().Be(double.NaN);
        order.Size.Should().Be(double.NaN);
        order.BspLiability.Should().Be(double.NaN);
        order.AveragePriceMatched.Should().Be(double.NaN);
        order.SizeMatched.Should().Be(double.NaN);
        order.SizeRemaining.Should().Be(double.NaN);
        order.SizeLapsed.Should().Be(double.NaN);
        order.SizeCancelled.Should().Be(double.NaN);
        order.SizeVoided.Should().Be(double.NaN);
    }

    [Fact]
    public void EnumFieldsDefaultToUnknown()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray());

        order.Side.Should().Be(Side.Unknown);
        order.Status.Should().Be(OrderStatus.Unknown);
        order.PersistenceType.Should().Be(PersistenceType.Unknown);
        order.OrderType.Should().Be(OrderType.Unknown);
    }

    [Fact]
    public void StringFieldsDefaultToNull()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray());

        order.LapsedStatusReasonCode.Should().BeNull();
        order.RegulatorAuthCode.Should().BeNull();
        order.RegulatorCode.Should().BeNull();
        order.OrderReference.Should().BeNull();
        order.StrategyReference.Should().BeNull();
    }

    [Fact]
    public void LongFieldsDefaultToZero()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray());

        order.PlacedDate.Should().Be(0);
        order.MatchedDate.Should().Be(0);
        order.CancelledDate.Should().Be(0);
        order.LapsedDate.Should().Be(0);
    }

    [Fact]
    public void AllFieldsCanBeSet()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray())
        {
            Price = 2.5,
            Size = 10.0,
            BspLiability = 100.0,
            Side = Side.Back,
            Status = OrderStatus.Executable,
            PersistenceType = PersistenceType.Persist,
            OrderType = OrderType.Limit,
            PlacedDate = 1700000000000,
            MatchedDate = 1700000001000,
            CancelledDate = 1700000002000,
            LapsedDate = 1700000003000,
            AveragePriceMatched = 2.75,
            SizeMatched = 5.0,
            SizeRemaining = 5.0,
            SizeLapsed = 0,
            SizeCancelled = 0,
            SizeVoided = 0,
        };

        order.Price.Should().Be(2.5);
        order.Size.Should().Be(10.0);
        order.BspLiability.Should().Be(100.0);
        order.Side.Should().Be(Side.Back);
        order.Status.Should().Be(OrderStatus.Executable);
        order.PersistenceType.Should().Be(PersistenceType.Persist);
        order.OrderType.Should().Be(OrderType.Limit);
        order.PlacedDate.Should().Be(1700000000000);
        order.MatchedDate.Should().Be(1700000001000);
        order.CancelledDate.Should().Be(1700000002000);
        order.LapsedDate.Should().Be(1700000003000);
        order.AveragePriceMatched.Should().Be(2.75);
        order.SizeMatched.Should().Be(5.0);
        order.SizeRemaining.Should().Be(5.0);
        order.SizeLapsed.Should().Be(0);
        order.SizeCancelled.Should().Be(0);
        order.SizeVoided.Should().Be(0);
    }

    [Fact]
    public void BetIdBytesReturnsCorrectSpan()
    {
        var order = new UnmatchedOrderCache("bet123"u8.ToArray());

        order.BetIdBytes.ToArray().Should().BeEquivalentTo("bet123"u8.ToArray());
    }

    [Fact]
    public void BetIdEqualsReturnsTrueForMatch()
    {
        var order = new UnmatchedOrderCache("bet123"u8.ToArray());

        order.BetIdEquals("bet123"u8).Should().BeTrue();
    }

    [Fact]
    public void BetIdEqualsReturnsFalseForMismatch()
    {
        var order = new UnmatchedOrderCache("bet123"u8.ToArray());

        order.BetIdEquals("bet999"u8).Should().BeFalse();
    }

    [Fact]
    public void ResetClearsAllFields()
    {
        var order = new UnmatchedOrderCache("bet1"u8.ToArray())
        {
            Price = 2.5,
            Size = 10.0,
            Side = Side.Back,
            Status = OrderStatus.Executable,
            PlacedDate = 1700000000000,
        };

        order.Reset("bet2"u8.ToArray());

        order.BetId.Should().Be("bet2");
        order.Price.Should().Be(double.NaN);
        order.Size.Should().Be(double.NaN);
        order.Side.Should().Be(Side.Unknown);
        order.Status.Should().Be(OrderStatus.Unknown);
        order.PlacedDate.Should().Be(0);
    }

    [Fact]
    public void BetIdIsLazilyDecoded()
    {
        var order = new UnmatchedOrderCache("lazyTest"u8.ToArray());

        // First access decodes
        var first = order.BetId;

        // Second access returns cached string (same reference)
        var second = order.BetId;

        first.Should().Be("lazyTest");
        ReferenceEquals(first, second).Should().BeTrue();
    }
}
