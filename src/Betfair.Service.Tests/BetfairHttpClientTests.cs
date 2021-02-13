using System;
using System.Net.Http.Headers;
using Xunit;

namespace Betfair.Service.Tests
{
    public class BetfairHttpClientTests : BetfairHttpClient
    {
        [Fact]
        public void TimeoutIsSetToThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30), Timeout);
        }

        [Fact]
        public void AcceptHeaderContainsApplicationJson()
        {
            Assert.Contains(
                new MediaTypeWithQualityHeaderValue("application/json"),
                DefaultRequestHeaders.Accept);
        }

        [Fact]
        public void ConnectionIsSetToKeepAlive()
        {
            Assert.Equal("keep-alive", DefaultRequestHeaders.Connection.ToString());
        }

        [Fact]
        public void AcceptGzipEncoding()
        {
            Assert.Contains(
                "gzip",
                DefaultRequestHeaders.AcceptEncoding.ToString(),
                StringComparison.InvariantCulture);
        }

        [Fact]
        public void AcceptDeflateEncoding()
        {
            Assert.Contains(
                "deflate",
                DefaultRequestHeaders.AcceptEncoding.ToString(),
                StringComparison.InvariantCulture);
        }
    }
}
