using System.Text;
using Betfair.Stream;

namespace Betfair.Tests.Stream.TestDoubles;

public class StreamClientStub : StreamClient
{
    private static readonly TcpClientSpy _tcpClient = new ("test.com", 999);

    public StreamClientStub()
    : base(_tcpClient)
    {
    }

    public override StreamWriter Writer => new StreamWriterSpy();

    public override StreamReader Reader { get; protected set; }

    public void SendLine(string line)
    {
        Reader = new StreamReader(
            new MemoryStream(
                Encoding.UTF8.GetBytes(line)));
    }
}
