using FluentAssertions;

namespace Betfair.Benchmarks.Tests;

public class ConnectionMessageTests
{
    [Fact]
    public void Utf8JsonSerializerReturnsConnectionMessage()
    {
        var sut = new ConnectionMessageBenchmarks();
        sut.Setup();

        var result = sut.Utf8();

        result.Operation.Should().Be("connection");
        result.ConnectionId.Should().Be("106-150324130517-1736998");
    }

    [Fact]
    public void CustomUtf8JsonReaderReturnsConnectionId()
    {
        var sut = new ConnectionMessageBenchmarks();
        sut.Setup();

        var result = sut.CustomUtf8JsonReader();

        System.Text.Encoding.UTF8.GetString(result).Should().Be("106-150324130517-1736998");
    }
}
