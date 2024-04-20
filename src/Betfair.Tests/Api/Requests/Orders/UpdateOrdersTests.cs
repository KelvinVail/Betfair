using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Requests.Orders;

public class UpdateOrdersTests
{
    [Fact]
    public void MarketIdShouldThrowWhenNull()
    {
        Action act = () => new UpdateOrders(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("marketId");
    }

    [Fact]
    public void InstructionsPropertyShouldAcceptNonNullValue()
    {
        var orders = new UpdateOrders("1.23456789") { Instructions = new List<UpdateInstruction> { new UpdateInstruction() } };
        orders.Instructions.Should().NotBeNull();
    }

    [Fact]
    public void CustomerRefPropertyShouldAcceptNonNullValue()
    {
        var orders = new UpdateOrders("1.23456789") { CustomerRef = "Test" };
        orders.CustomerRef.Should().Be("Test");
    }

    [Fact]
    public void MarketIdShouldBeSettable()
    {
        var orders = new UpdateOrders("1.23456789");
        orders.MarketId.Should().Be("1.23456789");
    }

    [Fact]
    public void InstructionsPropertyShouldAcceptNull()
    {
        var orders = new UpdateOrders("1.23456789") { Instructions = null };
        orders.Instructions.Should().BeNull();
    }

    [Fact]
    public void CustomerRefPropertyShouldAcceptNull()
    {
        var orders = new UpdateOrders("1.23456789") { CustomerRef = null };
        orders.CustomerRef.Should().BeNull();
    }

    [Fact]
    public void InstructionsPropertyShouldAcceptEmptyList()
    {
        var orders = new UpdateOrders("1.23456789") { Instructions = new List<UpdateInstruction>() };
        orders.Instructions.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeAotSerializable()
    {
        var orders = new UpdateOrders("1.23456789")
        {
            Instructions = new List<UpdateInstruction>
            {
                new ()
                {
                    BetId = "1",
                    NewPersistenceType = PersistenceType.Lapse,
                },
            },
            CustomerRef = "customer-ref",
        };

        var json = JsonSerializer.Serialize(orders, orders.GetContext());
        json.Should().Be("{\"marketId\":\"1.23456789\",\"instructions\":[{\"betId\":\"1\",\"newPersistenceType\":\"LAPSE\"}],\"customerRef\":\"customer-ref\"}");
    }
}
