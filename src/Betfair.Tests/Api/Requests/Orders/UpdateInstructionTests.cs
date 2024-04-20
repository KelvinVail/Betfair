using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Requests.Orders;

public class UpdateInstructionTests
{
    [Fact]
    public void BetIdPropertyShouldBeSettable()
    {
        var instruction = new UpdateInstruction { BetId = "1234" };
        instruction.BetId.Should().Be("1234");
    }

    [Fact]
    public void NewPersistenceTypePropertyShouldBeSettable()
    {
        var instruction = new UpdateInstruction { NewPersistenceType = PersistenceType.Lapse };
        instruction.NewPersistenceType.Should().Be(PersistenceType.Lapse);
    }

    [Fact]
    public void BetIdPropertyShouldBeEmptyByDefault()
    {
        var instruction = new UpdateInstruction();
        instruction.BetId.Should().Be(string.Empty);
    }

    [Fact]
    public void NewPersistenceTypePropertyShouldBeLapseByDefault()
    {
        var instruction = new UpdateInstruction();
        instruction.NewPersistenceType.Should().Be(PersistenceType.Lapse);
    }

    [Fact]
    public void ShouldBeSerializable()
    {
        var instruction = new UpdateInstruction { BetId = "1234", NewPersistenceType = PersistenceType.Lapse };

        var jsonTypeInfo = instruction.GetContext();
        var json = JsonSerializer.Serialize(instruction, jsonTypeInfo);

        json.Should().Be("{\"betId\":\"1234\",\"newPersistenceType\":\"LAPSE\"}");
    }
}
