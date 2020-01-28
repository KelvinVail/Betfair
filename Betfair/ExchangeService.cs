namespace Betfair
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal sealed class ExchangeService : IDisposable
    {
        private readonly ExchangeHttpClient client;

        private readonly ISession session;

        private readonly string apiEndpoint;

        internal ExchangeService(ISession session, string apiName)
        {
            this.session = session;
            this.client = new ExchangeHttpClient(new Uri($"https://api.betfair.com/exchange/{apiName.ToLower(CultureInfo.CurrentCulture)}/json-rpc/v1"));
            this.apiEndpoint = apiName == "betting" ? "Sports" : apiName;
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        internal ExchangeService WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }

        internal async Task<T> SendAsync<T>(string method)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            request.Headers.Add("X-Application", this.session.AppKey);
            var token = await this.session.GetSessionTokenAsync();
            request.Headers.Add("X-Authentication", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new ExchangeRequest(this.apiEndpoint, method)));
            var response = await this.client.SendAsync<ExchangeResponse<T>>(request);
            if (response.Error != null) throw new HttpRequestException(response.Error);
            return response.Result;
        }

        private sealed class ExchangeRequest
        {
            internal ExchangeRequest(string endpoint, string method)
            {
                this.Method = $"{endpoint}APING/v1.0/{method}";
                this.Id = 1;
                this.Jsonrpc = "2.0";
            }

            [JsonProperty(PropertyName = "jsonrpc")]
            internal string Jsonrpc { get; set; }

            [JsonProperty(PropertyName = "id")]
            internal int Id { get; set; }

            [JsonProperty(PropertyName = "method")]
            internal string Method { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        private sealed class ExchangeResponse<T>
        {
            [JsonProperty(PropertyName = "result")]
            internal T Result { get; set; }

            [JsonProperty(PropertyName = "error")]
            internal string Error { get; set; }
        }
    }
}
