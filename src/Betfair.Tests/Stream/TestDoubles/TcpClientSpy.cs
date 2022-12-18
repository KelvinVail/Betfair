using System.Net.Security;
using Betfair.Stream;

namespace Betfair.Tests.Stream.TestDoubles;

public sealed class TcpClientSpy : ITcpClient
{
    private readonly SslStreamStub _sslStream = new ();
    private bool _disposedValue;

    public TcpClientSpy(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public string Host { get; private set; }

    public int Port { get; private set; }

    public int ReceiveBufferSize { get; set; }

    public int SendTimeout { get; set; }

    public int ReceiveTimeout { get; set; }

    public int TimeGetSslStreamCalled { get; set; }

    public bool Connected { get; set; }

    public bool IsAuthenticated => _sslStream.IsAuthenticated;

    public void Connect(string host, int port)
    {
        Host = host;
        Port = port;
        Connected = true;
    }

    public void Close()
    {
        Connected = false;
    }

    public SslStream GetSslStream()
    {
        TimeGetSslStreamCalled++;

        return _sslStream;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            _sslStream.Dispose();

        _disposedValue = true;
    }

    private class SslStreamStub : SslStream
    {
        private bool _authenticated;

        public SslStreamStub()
            : base(new MemoryStream())
        {
        }

        public override bool CanRead { get; } = true;

        public override bool CanWrite { get; } = true;

        public override bool IsAuthenticated => _authenticated;

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override void AuthenticateAsClient(string targetHost) =>
            _authenticated = true;
    }
}