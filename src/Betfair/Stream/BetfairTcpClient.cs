using System.Net.Security;
using System.Net.Sockets;

namespace Betfair.Stream;

public class BetfairTcpClient : TcpClient, ITcpClient
{
    public SslStream GetSslStream() =>
        new (GetStream(), false);
}
