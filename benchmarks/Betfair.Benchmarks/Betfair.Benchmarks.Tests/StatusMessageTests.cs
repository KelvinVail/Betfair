using FluentAssertions;

namespace Betfair.Benchmarks.Tests;

public class StatusMessageTests
{
    [Fact]
    public void Utf8JsonSerializerReturnsConnectionMessage()
    {
        var sut = new StatusMessageBenchmarks();
        sut.Setup();

        var result = sut.Utf8();

        result.IsClosed.Should().BeFalse();
        result.Id.Should().Be(1);
    }

    [Fact]
    public void CustomerUtf8JsonReaderReturnsConnectionMessage()
    {
        var sut = new StatusMessageBenchmarks();
        sut.Setup();

        var result = sut.CustomUtf8Reader();

        result.Item1.Should().Be(1);
        result.Item2.Should().BeFalse();
    }
}
