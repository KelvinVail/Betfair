using Betfair.Stream;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Stream;

public class PipelineTests : IDisposable
{
    private readonly System.IO.Stream _stream = new MemoryStream();
    private readonly Pipeline _pipeline;
    private bool _disposedValue;

    public PipelineTests() =>
        _pipeline = new Pipeline(_stream);

    [Theory]
    [InlineData(1)]
    [InlineData(98765)]
    public async Task ObjectAreWrittenToTheStream(int id)
    {
        var anon = new { Id = id, Test = "Test " };

        await _pipeline.WriteLine(anon);

        var json = JsonSerializer.ToJsonString(anon, StandardResolver.CamelCase);
        ReadLineFrom(_stream).Should().Be(json);
    }

    [Fact]
    public async Task TheLastByteWrittenToTheStreamShouldBeAnEndOfLineChar()
    {
        var anon = new { Id = 1, Test = "Test " };

        await _pipeline.WriteLine(anon);

        _stream.Position = _stream.Length - 1;
        _stream.ReadByte().Should().Be((byte)'\n');
    }

    [Fact]
    public async Task ReadLinesShouldReturnAsSoonAsTheCancellationTokenIsCalled()
    {
        var anon = new { Id = 1, Test = "Test " };

        using var cts = new CancellationTokenSource();
        await _pipeline.WriteLine(anon);
        _stream.Position = 0;

        await cts.CancelAsync();
        var result = await _pipeline.ReadLines(cts.Token).ToListAsync(cancellationToken: cts.Token);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadLinesShouldReturnAllLinesWrittenToTheStream()
    {
        var anon1 = new { Id = 1, Test = "Test 1" };
        var anon2 = new { Id = 2, Test = "Test 2" };

        await _pipeline.WriteLine(anon1);
        await _pipeline.WriteLine(anon2);
        _stream.Position = 0;

        var result = await _pipeline.ReadLines(CancellationToken.None).ToListAsync();

        result.Should().HaveCount(2);
        result[0].Should().BeEquivalentTo(JsonSerializer.Serialize(anon1, StandardResolver.CamelCase));
        result[1].Should().BeEquivalentTo(JsonSerializer.Serialize(anon2, StandardResolver.CamelCase));
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _stream.Dispose();

        _disposedValue = true;
    }

    private static string? ReadLineFrom(System.IO.Stream stream)
    {
        stream.Position = 0;
        using var sr = new StreamReader(stream);

        return sr.ReadLine();
    }
}
