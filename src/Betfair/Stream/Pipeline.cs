namespace Betfair.Stream;

internal class Pipeline : IDisposable
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();
    private readonly BetfairTcpClient _tcp;
    private bool _disposedValue;

    public Pipeline(BetfairTcpClient tcp)
    {
        _tcp = tcp;
        _tcp.Configure();
        _stream = _tcp.GetAuthenticatedSslStream();
    }

    public Pipeline(System.IO.Stream stream)
    {
        _stream = stream;
        _tcp = new BetfairTcpClient();
    }

    public Task Write(object value) =>
        JsonSerializer.SerializeAsync(_stream, value, StandardResolver.AllowPrivateExcludeNullCamelCase)
            .ContinueWith(_ => _stream.WriteByte((byte)'\n'));

    public async IAsyncEnumerable<byte[]> Read()
    {
        Task writer = StreamToPipe(_stream, _pipe.Writer);
        await foreach (var line in ReadFromPipe(_pipe.Reader))
            yield return line;

        await writer;
    }

    public void Close() =>
        _tcp.Close();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _stream.Dispose();
            _tcp.Dispose();
        }

        _disposedValue = true;
    }

    private static async Task StreamToPipe(System.IO.Stream stream, PipeWriter writer)
    {
        const int minimumBufferSize = 512;

        int bytesRead;
        FlushResult result;
        do
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);

            bytesRead = await stream.ReadAsync(memory);
            writer.Advance(bytesRead);
            result = await writer.FlushAsync();
        }
        while (bytesRead != 0 && !result.IsCompleted);

        await writer.CompleteAsync();
    }

    private static async IAsyncEnumerable<byte[]> ReadFromPipe(PipeReader reader)
    {
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;

            SequencePosition? position;
            do
            {
                position = buffer.PositionOf((byte)'\n');
                if (position == null) continue;

                var bytes = buffer.Slice(0, position.Value).ToArray();
                yield return bytes;
                buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            }
            while (position != null);

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted) break;
        }

        await reader.CompleteAsync();
    }
}
