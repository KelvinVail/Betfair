#nullable enable
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

public class StreamClient : IDisposable
{
    private const string _hostName = "stream-api.betfair.com";
    private readonly ITcpClient _tcpClient;
    private string _connectionId;
    private StreamWriter _writer;
    private StreamReader _reader;
    private bool _disposedValue;

#pragma warning disable CS8618
    public StreamClient()
    {
        _tcpClient = new ExchangeStreamClient();
        Connect();
    }

    public StreamClient(ITcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        Connect();
    }
#pragma warning restore CS8618

    public virtual async Task SendLine(object value)
    {
        if (value is string str)
        {
            await _writer.WriteLineAsync(str);
            return;
        }

        var json = JsonSerializer.ToJsonString(value, StandardResolver.ExcludeNullCamelCase);
        await _writer.WriteLineAsync(json);
        await _writer.FlushAsync();
    }

    public virtual async Task<Result<Maybe<T>, ErrorResult>> ReadLine<T>()
    {
        var line = await _reader.ReadLineAsync();
        if (line is null) return Maybe<T>.None;
        return Maybe.From(JsonSerializer.Deserialize<T>(line, StandardResolver.ExcludeNullCamelCase));
    }

    public void Disconnect() =>
        _tcpClient.Close();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
        {
            _reader.Dispose();
            _writer.Dispose();
            _tcpClient.Close();
            _tcpClient.Dispose();
        }

        _disposedValue = true;
    }

    private static SslStream GetStream(ITcpClient client)
    {
        client.ReceiveBufferSize = 1024 * 1000 * 2;
        client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.Connect(_hostName, 443);

        var sslStream = client.GetSslStream();
        sslStream.AuthenticateAsClient(_hostName);

        return sslStream;
    }

    private void Connect()
    {
        var stream = GetStream(_tcpClient);

        _reader = new StreamReader(
            stream,
            Encoding.UTF8,
            false,
            _tcpClient.ReceiveBufferSize);

        _writer = new StreamWriter(stream, Encoding.UTF8);

        SetConnectionDetails();
    }

    private void SetConnectionDetails()
    {
        var line = _reader.ReadLine();
        if (line is null) return;
        var message = JsonSerializer.Deserialize<ConnectionMessage>(line, StandardResolver.ExcludeNullCamelCase);
        _connectionId = message.ConnectionId;
    }

    private sealed class ExchangeStreamClient : TcpClient, ITcpClient
    {
        public SslStream GetSslStream() =>
            new (GetStream(), false);
    }
}
