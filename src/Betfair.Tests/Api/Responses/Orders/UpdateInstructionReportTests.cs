using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Responses.Orders;

public class UpdateInstructionReportTests
{
    [Fact]
    public void StatusPropertyShouldAcceptNull()
    {
        var report = new UpdateInstructionReport { Status = null };
        report.Status.Should().BeNull();
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNull()
    {
        var report = new UpdateInstructionReport { ErrorCode = null };
        report.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void InstructionPropertyShouldAcceptNull()
    {
        var report = new UpdateInstructionReport { Instruction = null };
        report.Instruction.Should().BeNull();
    }

    [Fact]
    public void StatusPropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateInstructionReport { Status = "Test" };
        report.Status.Should().Be("Test");
    }

    [Fact]
    public void ErrorCodePropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateInstructionReport { ErrorCode = "Test" };
        report.ErrorCode.Should().Be("Test");
    }

    [Fact]
    public void InstructionPropertyShouldAcceptNonNullValue()
    {
        var report = new UpdateInstructionReport { Instruction = new UpdateInstruction() };
        report.Instruction.Should().NotBeNull();
    }

    [Fact]
    public void ShouldBeDeserializable()
    {
        const string json =
            """
            {
              "status": "SUCCESS",
              "instruction": {
                "betId": "123456789",
                "newPersistenceType": "LAPSE"
              }
            }
            """;

        var report = JsonSerializer.Deserialize(json, SerializerContext.Default.UpdateInstructionReport);

        report.Should().NotBeNull();
        report!.Status.Should().Be("SUCCESS");
        report.ErrorCode.Should().BeNull();
        report.Instruction.Should().NotBeNull();
        report.Instruction!.BetId.Should().Be("123456789");
        report.Instruction!.NewPersistenceType.Should().Be(PersistenceType.Lapse);
    }
}
