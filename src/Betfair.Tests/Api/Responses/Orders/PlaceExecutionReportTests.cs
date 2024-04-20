using Betfair.Api.Responses.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Responses.Orders;

public class PlaceExecutionReportTests
{
    [Fact]
    public void MarketIdPropertyShouldBeEmptyByDefault()
    {
        var report = new PlaceExecutionReport();
        report.MarketId.Should().Be(string.Empty);
    }

    [Fact]
    public void StatusPropertyShouldBeEmptyByDefault()
    {
        var report = new PlaceExecutionReport();
        report.Status.Should().Be(string.Empty);
    }

    [Fact]
    public void ErrorCodePropertyShouldBeNullByDefault()
    {
        var report = new PlaceExecutionReport();
        report.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void CustomerRefPropertyShouldBeNullByDefault()
    {
        var report = new PlaceExecutionReport();
        report.CustomerRef.Should().BeNull();
    }

    [Fact]
    public void InstructionReportsPropertyShouldBeNullByDefault()
    {
        var report = new PlaceExecutionReport();
        report.InstructionReports.Should().BeNull();
    }

    [Fact]
    public void MarketIdPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceExecutionReport { MarketId = "Test" };
        report.MarketId.Should().Be("Test");
    }

    [Fact]
    public void StatusPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceExecutionReport { Status = "Test" };
        report.Status.Should().Be("Test");
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceExecutionReport { ErrorCode = "Test" };
        report.ErrorCode.Should().Be("Test");
    }

    [Fact]
    public void CustomerRefPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceExecutionReport { CustomerRef = "Test" };
        report.CustomerRef.Should().Be("Test");
    }

    [Fact]
    public void InstructionReportsPropertyShouldAcceptNonNullValue()
    {
        var report = new PlaceExecutionReport { InstructionReports = new List<PlaceInstructionReport> { new () } };
        report.InstructionReports.Should().NotBeNull();
    }

    [Fact]
    public void ShouldBeDeserializable()
    {
        const string json = "{\"marketId\":\"1.23456789\",\"status\":\"SUCCESS\",\"errorCode\":\"INVALID_ACCOUNT_STATE\",\"customerRef\":\"abc123\",\"instructionReports\":[{\"status\":\"SUCCESS\",\"errorCode\":\"INVALID_ACCOUNT_STATE\",\"instruction\":{\"selectionId\":12345678,\"handicap\":1.5,\"limitOrder\":{\"size\":2.0,\"price\":3.0,\"persistenceType\":\"LAPSE\"}}}],\"customerStrategyRef\":\"def456\"}";

        var report = JsonSerializer.Deserialize(json, SerializerContext.Default.PlaceExecutionReport);

        report!.MarketId.Should().Be("1.23456789");
        report.Status.Should().Be("SUCCESS");
        report.ErrorCode.Should().Be("INVALID_ACCOUNT_STATE");
        report.CustomerRef.Should().Be("abc123");
        report.InstructionReports.Should().NotBeNull();
        report.InstructionReports.Should().HaveCount(1);
        report.InstructionReports![0].Status.Should().Be("SUCCESS");
        report.InstructionReports![0].ErrorCode.Should().Be("INVALID_ACCOUNT_STATE");
        report.InstructionReports![0].Instruction.Should().NotBeNull();
        report.InstructionReports![0].Instruction!.SelectionId.Should().Be(12345678);
        report.InstructionReports![0].Instruction!.Handicap.Should().Be(1.5);
        report.InstructionReports![0].Instruction!.LimitOrder.Should().NotBeNull();
        report.InstructionReports![0].Instruction!.LimitOrder!.Size.Should().Be(2.0);
        report.InstructionReports![0].Instruction!.LimitOrder!.Price.Should().Be(3.0);
        report.InstructionReports![0].Instruction!.LimitOrder!.PersistenceType.Should().Be(PersistenceType.Lapse);
    }
}
