namespace Betfair.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Utf8Json;
    using Utf8Json.Resolvers;

    internal sealed class ExchangeClient : IDisposable
    {
        private readonly Uri baseAddress;

        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private HttpClient httpClient;

        private HttpClientHandler handler;

        internal ExchangeClient(Uri baseAddress)
        {
            this.baseAddress = baseAddress;
            this.handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                CheckCertificateRevocationList = true,
            };
            this.httpClient = this.Configure(new HttpClient(this.handler));
        }

        public void Dispose()
        {
            this.disposables.ForEach(d => d.Dispose());
            ((IDisposable)this.httpClient).Dispose();
            this.handler.Dispose();
        }

        internal void WithHandler(HttpClientHandler newHandler)
        {
            this.handler = newHandler ?? throw new ArgumentNullException(nameof(newHandler));
            this.handler.CheckCertificateRevocationList = true;
            this.handler.AutomaticDecompression = DecompressionMethods.GZip;
            this.httpClient = this.Configure(new HttpClient(this.handler));
        }

        internal async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            this.disposables.Add(request);
            var response = await this.httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"{response.StatusCode}");
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, StandardResolver.AllowPrivateExcludeNull);
        }

        private HttpClient Configure(HttpClient client)
        {
            client.BaseAddress = this.baseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return client;
        }
    }
}
