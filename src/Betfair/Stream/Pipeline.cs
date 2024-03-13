namespace Betfair.Stream;

internal class Pipeline : IPipeline
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();

    public Pipeline(System.IO.Stream stream) =>
        _stream = stream;

    public async Task WriteLine(object value)
    {
        var context = SerializerContextSwitch.GetContext(value);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, context);
        _stream.Write(bytes);
        _stream.WriteByte((byte)'\n');
    }

    public async IAsyncEnumerable<byte[]> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _ = CopyDataFromSteamToPipeAsync(_stream, _pipe.Writer, cancellationToken);
        await foreach (var line in ReadPipeAsync(_pipe.Reader, cancellationToken))
            yield return line.Slice(0, line.Length).ToArray();
    }

    private static async Task CopyDataFromSteamToPipeAsync(
        System.IO.Stream stream,
        PipeWriter writer,
        CancellationToken cancellationToken)
    {
        const int minimumBufferSize = 512;

        while (true)
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);
            int bytesRead;
            try
            {
                bytesRead = await stream.ReadAsync(memory, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (bytesRead == 0) break;

            writer.Advance(bytesRead);
            FlushResult result = await writer.FlushAsync(cancellationToken);

            if (result.IsCompleted) break;
        }

        await writer.CompleteAsync();
    }

    private static async IAsyncEnumerable<ReadOnlySequence<byte>> ReadPipeAsync(PipeReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (true)
        {
            ReadResult result;
            try
            {
                result = await reader.ReadAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

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
