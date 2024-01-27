namespace Betfair.Stream;

internal class Pipeline
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();

    public Pipeline(System.IO.Stream stream) =>
        _stream = stream;

    public Task Write(object value) =>
        JsonSerializer.SerializeAsync(_stream, value, StandardResolver.AllowPrivateExcludeNullCamelCase)
            .ContinueWith(_ => _stream.WriteByte((byte)'\n'))
            .ContinueWith(_ => FillPipeAsync(_stream, _pipe.Writer));

    public async IAsyncEnumerable<byte[]> Read()
    {
        await foreach (var line in ReadPipeAsync(_pipe.Reader))
            yield return line.Slice(0, line.Length).ToArray();
    }

    private async Task FillPipeAsync(System.IO.Stream stream, PipeWriter writer)
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
