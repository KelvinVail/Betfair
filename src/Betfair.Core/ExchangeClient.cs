using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Core
{
    internal sealed class ExchangeClient : IDisposable
    {
        private readonly Uri _baseAddress;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private HttpClient _httpClient;
        private HttpClientHandler _handler;

        internal ExchangeClient(string urlPrefix)
        {
            _baseAddress = new Uri($"https://{urlPrefix}.betfair.com/");
            _handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                CheckCertificateRevocationList = true,
            };
            _httpClient = Configure(new HttpClient(_handler));
        }

        public void Dispose()
        {
            _disposables.ForEach(d => d.Dispose());
            ((IDisposable)_httpClient).Dispose();
            _handler.Dispose();
        }

        internal void WithHandler(HttpClientHandler newHandler)
        {
            _handler = newHandler ?? throw new ArgumentNullException(nameof(newHandler));
            _handler.CheckCertificateRevocationList = true;
            _handler.AutomaticDecompression = DecompressionMethods.GZip;
            _httpClient = Configure(new HttpClient(_handler));
        }

        internal async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            _disposables.Add(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"{response.StatusCode}");
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, StandardResolver.AllowPrivateExcludeNull);
        }

        private HttpClient Configure(HttpClient client)
        {
            client.BaseAddress = _baseAddress;
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
