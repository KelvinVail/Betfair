using System.Buffers;
using System.IO.Pipelines;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

public class Pipeline
{
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();

    public Pipeline(System.IO.Stream stream) =>
        _stream = stream;

    public virtual Task Write(object value) =>
        JsonSerializer.SerializeAsync(_stream, value, StandardResolver.ExcludeNullCamelCase)
            .ContinueWith(_ => _stream.WriteByte((byte)'\n'));

    public virtual async IAsyncEnumerable<ChangeMessage> Read()
    {
        Task writer = FillPipe(_stream, _pipe.Writer);
        await foreach (var line in ReadPipe(_pipe.Reader))
        {
            yield return line;
            if (writer.IsCompleted) yield break;
        }
    }

    private static async Task FillPipe(System.IO.Stream stream, PipeWriter writer)
    {
        const int minimumBufferSize = 512;

        while (true)
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);

            var bytesRead = await stream.ReadAsync(memory);
            if (bytesRead == 0) break;
            writer.Advance(bytesRead);
            var result = await writer.FlushAsync();
            if (result.IsCompleted) break;
        }

        await writer.CompleteAsync();
    }

    private static async IAsyncEnumerable<ChangeMessage> ReadPipe(PipeReader reader)
    {
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;
            SequencePosition? position = null;

            do
            {
                position = buffer.PositionOf((byte)'\n');
                if (position == null)
                    continue;

                var bytes = buffer.Slice(0, position.Value).ToArray();
                yield return JsonSerializer.Deserialize<ChangeMessage>(bytes, StandardResolver.ExcludeNullCamelCase);
                buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            }
            while (position != null);

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted) break;
        }

        await reader.CompleteAsync();
    }
}
