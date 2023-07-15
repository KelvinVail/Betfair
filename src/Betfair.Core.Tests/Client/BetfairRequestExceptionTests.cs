using System.Text.Json;
using Betfair.Core.Client;

namespace Betfair.Core.Tests.Client;

public class BetfairRequestExceptionTests
{
    [Fact]
    public void CanBeCreatedUsingParameterlessConstructor()
    {
        var ex = new BetfairRequestException();

        ex.Message.Should().Be("Exception of type 'Betfair.Core.Client.BetfairRequestException' was thrown.");
    }

    [Theory]
    [InlineData("message")]
    [InlineData("other")]
    public void CanBeCreatedUsingACustomMessage(string message)
    {
        var ex = new BetfairRequestException(message);

        ex.Message.Should().Be(message);
    }

    [Theory]
    [InlineData("message")]
    [InlineData("other")]
    public void CanBeCreatedUsingACustomMessageAndAnInnerException(string message)
    {
        var inner = new Exception("inner");
        var ex = new BetfairRequestException(message, inner);

        ex.Message.Should().Be(message);
        ex.InnerException.Should().Be(inner);
        ex.InnerException?.Message.Should().Be("inner");
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public void CanBeCreatedWithAnHttpStatusCode(HttpStatusCode status)
    {
        var ex = new BetfairRequestException(status);

        ex.StatusCode.Should().Be(status);
    }

    [Fact]
    public void CanBeSerialized()
    {
        var ex = new BetfairRequestException(HttpStatusCode.BadRequest);

        var serialized = JsonSerializer.SerializeToUtf8Bytes(ex);
        var deserialized = JsonSerializer.Deserialize<BetfairRequestException>(serialized);

        deserialized.Should().BeEquivalentTo(ex);
    }
}
