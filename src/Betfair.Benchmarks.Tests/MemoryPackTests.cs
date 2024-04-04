using FluentAssertions;

namespace Betfair.Benchmarks.Tests;

public class MemoryPackTests
{
    [Fact]
    public void MemoryPackReturnsInitialImage()
    {
        var sut = new MemoryPackBenchmarks();
        sut.Setup();

        var result = sut.MemoryPackBenchmark();

        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.InitialClock.Should().Be("GpSZ6vkEG/Wb6ukEFOHyyPQE");
    }

    [Fact(Skip = "X")]
    public void MemoryPackReturnsInitialImageFromFile()
    {
        var sut = new MemoryPackBenchmarks();
        sut.Setup();

        var result = sut.MemoryPackFromFile();

        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.InitialClock.Should().Be("GpSZ6vkEG/Wb6ukEFOHyyPQE");
    }
}
