using Betfair.Api.Betting.Endpoints.UpdateOrders.Responses;
using Betfair.Api.Betting.Enums;

namespace Betfair.Tests.Api.Responses.Orders;

public class UpdateExecutionReportTests
{
    [Fact]
    public void CustomerRefPropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateExecutionReport { CustomerRef = "Test" };
        report.CustomerRef.Should().Be("Test");
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateExecutionReport { ErrorCode = "Test" };
        report.ErrorCode.Should().Be("Test");
    }

    [Fact]
    public void MarketIdPropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateExecutionReport { MarketId = "Test" };
        report.MarketId.Should().Be("Test");
    }

    [Fact]
    public void InstructionReportsPropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateExecutionReport { InstructionReports = new List<UpdateInstructionReport> { new () } };
        report.InstructionReports.Should().NotBeNull();
    }

    [Fact]
    public void CustomerRefPropertyShouldAcceptNull()
    {
        var report = new UpdateExecutionReport { CustomerRef = null };
        report.CustomerRef.Should().BeNull();
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNull()
    {
        var report = new UpdateExecutionReport { ErrorCode = null };
        report.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void MarketIdPropertyShouldAcceptNull()
    {
        var report = new UpdateExecutionReport { MarketId = null };
        report.MarketId.Should().BeNull();
    }

    [Fact]
    public void InstructionReportsPropertyShouldAcceptNull()
    {
        var report = new UpdateExecutionReport { InstructionReports = null };
        report.InstructionReports.Should().BeNull();
    }

    [Fact]
    public void InstructionReportsPropertyShouldAcceptEmptyList()
    {
        var report = new UpdateExecutionReport { InstructionReports = new List<UpdateInstructionReport>() };
        report.InstructionReports.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeDeserializable()
    {
        const string json = "{\"customerRef\":\"Test\",\"status\":\"SUCCESS\",\"errorCode\":\"INVALID_ACCOUNT_STATE\",\"marketId\":\"1.2345\",\"instructionReports\":[{\"status\":\"SUCCESS\",\"errorCode\":\"INVALID_ACCOUNT_STATE\",\"instruction\":{\"betId\":\"1.2345\",\"newPersistenceType\":\"LAPSE\"}}]}";

        var report = JsonSerializer.Deserialize(json, SerializerContext.Default.UpdateExecutionReport);

        report.Should().NotBeNull();
        report!.CustomerRef.Should().Be("Test");
        report.Status.Should().Be("SUCCESS");
        report.ErrorCode.Should().Be("INVALID_ACCOUNT_STATE");
        report.MarketId.Should().Be("1.2345");
        report.InstructionReports.Should().NotBeNull();
        report.InstructionReports.Should().HaveCount(1);
        report.InstructionReports![0].Status.Should().Be("SUCCESS");
        report.InstructionReports[0].ErrorCode.Should().Be("INVALID_ACCOUNT_STATE");
        report.InstructionReports[0].Instruction.Should().NotBeNull();
        report.InstructionReports[0].Instruction!.BetId.Should().Be("1.2345");
        report.InstructionReports[0].Instruction!.NewPersistenceType.Should().Be(PersistenceType.Lapse);
    }
}
