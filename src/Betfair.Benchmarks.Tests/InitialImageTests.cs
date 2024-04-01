using System.Text;
using FluentAssertions;

namespace Betfair.Benchmarks.Tests;

public class InitialImageTests
{
    [Fact]
    public void Utf8JsonSerializerReturnsConnectionMessage()
    {
        var sut = new InitialImageBenchmarks();
        sut.Setup();

        var result = sut.Utf8();

        result.Operation.Should().Be("mcm");
        result.Id.Should().Be(2);
        result.InitialClock.Should().Be("GpSZ6vkEG/Wb6ukEFOHyyPQE");
        result.Clock.Should().Be("AAAAAAAA");
        result.ConflateMs.Should().Be(0);
        result.HeartbeatMs.Should().Be(5000);
        result.PublishTime.Should().Be(1710507917481);
        result.ChangeType.Should().Be("SUB_IMAGE");
        result.MarketChanges.Should().HaveCount(1);

        var mc = result.MarketChanges[0];
        mc.MarketId.Should().Be("1.226122413");
        var md = mc.MarketDefinition;
        md.BspMarket.Should().BeTrue();
        md.TurnInPlayEnabled.Should().BeTrue();
        md.PersistenceEnabled.Should().BeTrue();
        md.MarketBaseRate.Should().Be(5);
        md.BettingType.Should().Be("ODDS");

        md.Runners.Should().HaveCount(10);
        md.Runners[0].Status.Should().Be("ACTIVE");

        mc.RunnerChanges.Should().HaveCount(10);
        mc.RunnerChanges[1].TotalMatched.Should().Be(306.46);
        mc.RunnerChanges[1].BestAvailableToBack[0].Should().HaveCount(3);
    }

    [Fact]
    public void CustomUtf8JsonSerializerReturnsConnectionMessage()
    {
        var sut = new InitialImageBenchmarks();
        sut.Setup();

        var result = sut.CustomUtf8Reader();

        result.Id.Should().Be(2);
        result.InitialClock.Should().Be("GpSZ6vkEG/Wb6ukEFOHyyPQE");
        //result.Clock.Should().Be("AAAAAAAA");
        //result.ConflateMs.Should().Be(0);
        //result.HeartbeatMs.Should().Be(5000);
        //result.PublishTime.Should().Be(1710507917481);
        //result.ChangeType.Should().Be("SUB_IMAGE");
        //result.MarketChanges.Should().HaveCount(1);

        //var mc = result.MarketChanges[0];
        //mc.MarketId.Should().Be("1.226122413");
        //var md = mc.MarketDefinition;
        //md.BspMarket.Should().BeTrue();
        //md.TurnInPlayEnabled.Should().BeTrue();
        //md.PersistenceEnabled.Should().BeTrue();
        //md.MarketBaseRate.Should().Be(5);
        //md.BettingType.Should().Be("ODDS");

        //md.Runners.Should().HaveCount(10);
        //md.Runners[0].Status.Should().Be("ACTIVE");

        //mc.RunnerChanges.Should().HaveCount(10);
        //mc.RunnerChanges[1].TotalMatched.Should().Be(306.46);
        //mc.RunnerChanges[1].BestAvailableToBack[0].Should().HaveCount(3);
    }
}
