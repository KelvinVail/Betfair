using Betfair.Stream;
using Betfair.Stream.Messages;
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
        var auth = new Authentication(id, "sessionToken", "appKey");

        await _pipeline.WriteLine(auth);

        var json = System.Text.Json.JsonSerializer.Serialize(auth);
        ReadLineFrom(_stream).Should().Be(json);
    }

    [Fact]
    public async Task TheLastByteWrittenToTheStreamShouldBeAnEndOfLineChar()
    {
        var auth = new Authentication(1, "sessionToken", "appKey");

        await _pipeline.WriteLine(auth);

        _stream.Position = _stream.Length - 1;
        _stream.ReadByte().Should().Be((byte)'\n');
    }

    [Fact]
    public async Task ReadLinesShouldReturnAsSoonAsTheCancellationTokenIsCalled()
    {
        var auth = new Authentication(1, "sessionToken", "appKey");
        using var cts = new CancellationTokenSource();
        await _pipeline.WriteLine(auth);
        _stream.Position = 0;

        await cts.CancelAsync();
        var result = await _pipeline.ReadLines(cts.Token).ToListAsync(cancellationToken: cts.Token);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadLinesShouldReturnAllLinesWrittenToTheStream()
    {
        var auth1 = new Authentication(1, "sessionToken", "appKey");
        var auth2 = new Authentication(2, "sessionToken", "appKey");
        await _pipeline.WriteLine(auth1);
        await _pipeline.WriteLine(auth2);
        _stream.Position = 0;

        var result = await _pipeline.ReadLines(CancellationToken.None).ToListAsync();

        result.Should().HaveCount(2);
        result[0].Should().BeEquivalentTo(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(auth1));
        result[1].Should().BeEquivalentTo(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(auth2));
    }

    [Fact]
    public async Task ReadLinesShouldNotReturnAnySequenceForLinesThatDoNotEndInNewLineChar()
    {
        var stream = new MemoryStream("Line\nNoNewlineHere"u8.ToArray());
        var pipeline = new Pipeline(stream);

        var asyncEnumerable = pipeline.ReadLines(default);

        var lines = await asyncEnumerable.ToListAsync();
        lines.Should().ContainSingle();
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
