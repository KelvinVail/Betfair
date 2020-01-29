namespace Betfair.Streaming
{
    using System;

    public class MarketSubscription
    {
        private readonly ISession session;
        private ITcpClient tcpClient;

        public MarketSubscription(ISession session)
        {
            this.session = session;
        }

        public MarketSubscription WithTcpClient(ITcpClient client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            this.tcpClient = SetTcpClient(client);
            return this;
        }

        private static ITcpClient SetTcpClient(ITcpClient client)
        {
            client.ReceiveBufferSize = 1024 * 1000 * 2;
            client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.Connect("stream-api.betfair.com", 443);
            return client;
        }
    }
}
