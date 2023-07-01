using System.Buffers;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

internal class Pipeline : IDisposable
{
    private const string _hostName = "stream-api.betfair.com";
    private readonly System.IO.Stream _stream;
    private readonly Pipe _pipe = new ();
    private readonly TcpClient _tcp = new ();
    private bool _disposedValue;

    public Pipeline()
    {
        _tcp.ReceiveBufferSize = 1024 * 1000 * 2;
        _tcp.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        _tcp.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        _tcp.Connect(_hostName, 443);

        var sslStream = new SslStream(_tcp.GetStream(), false);
        sslStream.AuthenticateAsClient(_hostName);

        _stream = sslStream;
    }

    public Pipeline(System.IO.Stream stream) =>
        _stream = stream;

    public virtual Task Write(object value) =>
        JsonSerializer.SerializeAsync(_stream, value, StandardResolver.AllowPrivateExcludeNullCamelCase)
            .ContinueWith(_ => _stream.WriteByte((byte)'\n'));

    public virtual async IAsyncEnumerable<byte[]> Read()
    {
        Task writer = FillPipe(_stream, _pipe.Writer);
        await foreach (var line in ReadPipe(_pipe.Reader))
        {
            yield return line;
            if (writer.IsCompleted) yield break;
        }
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
            _stream.Dispose();
            _tcp.Dispose();
        }

        _disposedValue = true;
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

    private static async IAsyncEnumerable<byte[]> ReadPipe(PipeReader reader)
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
