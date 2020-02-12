﻿namespace Betfair
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal sealed class ExchangeHttpClient : IDisposable
    {
        private readonly Uri baseAddress;

        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private HttpClient httpClient;

        private HttpClientHandler handler;

        internal ExchangeHttpClient(Uri baseAddress)
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

        internal ExchangeHttpClient WithHandler(HttpClientHandler newHandler)
        {
            this.handler = newHandler ?? throw new ArgumentNullException(nameof(newHandler));
            this.handler.CheckCertificateRevocationList = true;
            this.handler.AutomaticDecompression = DecompressionMethods.GZip;
            this.httpClient = this.Configure(new HttpClient(this.handler));
            return this;
        }

        internal async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            this.disposables.Add(request);
            var response = await this.httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"{response.StatusCode}");
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        private HttpClient Configure(HttpClient client)
        {
            client.BaseAddress = this.baseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return client;
        }
    }
}