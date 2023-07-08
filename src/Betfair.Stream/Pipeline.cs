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

    public Task Write(object value) =>
        JsonSerializer.SerializeAsync(_stream, value, StandardResolver.AllowPrivateExcludeNullCamelCase)
            .ContinueWith(_ => _stream.WriteByte((byte)'\n'));

    public async IAsyncEnumerable<byte[]> Read()
    {
        Task writer = FillPipe(_stream, _pipe.Writer);
        await foreach (var line in ReadPipe(_pipe.Reader))
            yield return line;

        await writer;
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

    private static async IAsyncEnumerable<byte[]> ReadPipe(PipeReader reader)
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
