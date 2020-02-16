namespace Betfair
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Utf8Json;
    using Utf8Json.Resolvers;

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
            var request = await this.GetRequest();
            request.Content = new StringContent(new ExchangeRequest(this.apiEndpoint, method).ToJson());
            return await this.GetResult<T>(request);
        }

        internal async Task<T> SendParametersAsync<T>(string method, dynamic parameters)
        {
            var request = await this.GetRequest();
            var content = new ExchangeRequest(this.apiEndpoint, method)
            {
                Params = parameters,
            };

            request.Content = new StringContent(content.ToJson());
            return await this.GetResult<T>(request);
        }

        private async Task<HttpRequestMessage> GetRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            request.Headers.Add("X-Application", this.session.AppKey);
            var token = await this.session.GetTokenAsync();
            request.Headers.Add("X-Authentication", token);
            return request;
        }

        private async Task<T> GetResult<T>(HttpRequestMessage request)
        {
            var response = await this.client.SendAsync<ExchangeResponse<T>>(request);
            if (response.Error != null) throw new HttpRequestException(response.Error);
            return response.Result;
        }

        [DataContract]
        private sealed class ExchangeRequest
        {
            internal ExchangeRequest(string endpoint, string method)
            {
                this.Method = $"{endpoint}APING/v1.0/{method}";
                this.Id = 1;
                this.Jsonrpc = "2.0";
            }

            [DataMember(Name = "jsonrpc", EmitDefaultValue = false)]
            internal string Jsonrpc { get; set; }

            [DataMember(Name = "id", EmitDefaultValue = false)]
            internal int Id { get; set; }

            [DataMember(Name = "method", EmitDefaultValue = false)]
            internal string Method { get; set; }

            [DataMember(Name = "params", EmitDefaultValue = false)]
            internal dynamic Params { get; set; }

            internal string ToJson()
            {
                return JsonSerializer.ToJsonString(this, StandardResolver.AllowPrivateExcludeNull);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        [DataContract]
        private sealed class ExchangeResponse<T>
        {
            [DataMember(Name = "result", EmitDefaultValue = false)]
            internal T Result { get; set; }

            [DataMember(Name = "error", EmitDefaultValue = false)]
            internal string Error { get; set; }
        }
    }
}
