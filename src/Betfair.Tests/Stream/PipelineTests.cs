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

        await _pipeline.WriteLines(anon, default);

        var json = JsonSerializer.ToJsonString(anon, StandardResolver.CamelCase);
        ReadLineFrom(_stream).Should().Be(json);
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
