namespace Betfair.Stream;

internal class Pipeline : IPipeline
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();

    public Pipeline(System.IO.Stream stream) =>
        _stream = stream;

    public async Task WriteLine(object value)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, value.GetInternalContext());
        await _stream.WriteAsync(bytes);
        _stream.WriteByte((byte)'\n');
    }

    public async IAsyncEnumerable<byte[]> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _ = CopyDataFromStreamToPipeAsync(_stream, _pipe.Writer, cancellationToken);
        await foreach (var line in ReadPipeAsync(_pipe.Reader, cancellationToken))
            yield return line.Slice(0, line.Length).ToArray();
    }

    /// <summary>
    /// Zero-copy line reading. Invokes the processor delegate for each line
    /// directly from the pipe buffer without allocating a byte[] copy.
    /// Uses TryRead before ReadAsync to avoid async state machine overhead
    /// when data is already buffered (common during message bursts).
    /// </summary>
    /// <param name="lineProcessor">The delegate invoked for each complete line read from the pipe.</param>
    /// <param name="cancellationToken">A token to cancel the processing loop.</param>
    /// <returns>A task that completes when the stream ends or cancellation is requested.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Pipeline read loop with fast-path TryRead — inherent branching for performance.")]
    internal async Task ProcessLines(ReadOnlySpanAction<byte> lineProcessor, CancellationToken cancellationToken)
    {
        _ = CopyDataFromStreamToPipeAsync(_stream, _pipe.Writer, cancellationToken);
        var reader = _pipe.Reader;

        while (true)
        {
            ReadResult result;

            // Fast path: if data is already buffered, avoid async state machine entirely
            if (!reader.TryRead(out result))
            {
                try
                {
                    result = await reader.ReadAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            var buffer = result.Buffer;

            while (TryReadLine(ref buffer, out var line))
            {
                if (line.IsSingleSegment)
                {
                    lineProcessor.Invoke(line.FirstSpan);
                }
                else
                {
                    // Multi-segment: must copy to contiguous buffer
                    var length = (int)line.Length;
                    var rented = System.Buffers.ArrayPool<byte>.Shared.Rent(length);
                    line.CopyTo(rented);
                    lineProcessor.Invoke(rented.AsSpan(0, length));
                    System.Buffers.ArrayPool<byte>.Shared.Return(rented);
                }
            }

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted) break;
        }

        await reader.CompleteAsync();
    }

    private static async Task CopyDataFromStreamToPipeAsync(
        System.IO.Stream stream,
        PipeWriter writer,
        CancellationToken cancellationToken)
    {
        const int minimumBufferSize = 4096;

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
