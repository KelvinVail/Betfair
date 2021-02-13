using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Betfair.Service
{
    public class BetfairHttpClient : HttpClient
    {
        public BetfairHttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30);
            DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.Add("Connection", "keep-alive");
            DefaultRequestHeaders.AcceptEncoding.Add(
                new StringWithQualityHeaderValue("gzip"));
            DefaultRequestHeaders.AcceptEncoding.Add(
                new StringWithQualityHeaderValue("deflate"));
        }
    }
}
