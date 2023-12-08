namespace Betfair.Stream;

internal class Pipeline : IDisposable
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();
    private readonly BetfairTcpClient _tcp;
    private bool _isClosing = false;
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
        Task writing = FillPipeAsync(_stream, _pipe.Writer);
        await foreach (var line in ReadPipeAsync(_pipe.Reader))
            yield return line.Slice(0, line.Length).ToArray();

        await writing;
    }

    public void Close()
    {
        _tcp.Client.Shutdown(SocketShutdown.Both);
        _stream.Close();
        _tcp.Close();
    }

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
            _tcp.Client.Shutdown(SocketShutdown.Both);
            _stream.Dispose();
            _tcp.Dispose();
        }

        _disposedValue = true;
    }

    private static async Task FillPipeAsync(System.IO.Stream stream, PipeWriter writer)
    {
        const int minimumBufferSize = 512;

        while (true)
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);
            try
            {
                int bytesRead = await stream.ReadAsync(memory);
                if (bytesRead == 0) break;

                writer.Advance(bytesRead);
            }
            catch (SocketException)
            {
                break;
            }
            catch (IOException)
            {
                break;
            }

            FlushResult result = await writer.FlushAsync();

            if (result.IsCompleted) break;
        }

        await writer.CompleteAsync();
    }

    private static async IAsyncEnumerable<ReadOnlySequence<byte>> ReadPipeAsync(PipeReader reader)
    {
        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                yield return line;

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted) break;
        }

        await reader.CompleteAsync();
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        SequencePosition? position = buffer.PositionOf((byte)'\n');

        if (position == null)
        {
            line = default;
            return false;
        }

        line = buffer.Slice(0, position.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
        return true;
    }
}
