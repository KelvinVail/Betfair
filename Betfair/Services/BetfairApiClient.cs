namespace Betfair.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    using Marvin.StreamExtensions;

    using Newtonsoft.Json;

    /// <summary>
    /// The betfair API betfairClient.
    /// </summary>
    internal sealed class BetfairApiClient : IBetfairApiClient
    {
        /// <summary>
        /// Betfair API base address.
        /// </summary>
        private const string BaseAddress = "https://api.betfair.com/";

        /// <summary>
        /// The Http betfairClient.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Betfair app key.
        /// </summary>
        private readonly string appKey;

        /// <summary>
        /// A Betfair session token.
        /// </summary>
        private readonly string sessionToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="BetfairApiClient"/> class. 
        /// Use this constructor if you already have a valid session token.
        /// You do not need to logon to Betfair or provide your username and password 
        /// in a separate action if you already have a valid session token.
        /// </summary>
        /// <param name="appKey">
        /// A Betfair app key.
        /// </param>
        /// <param name="sessionToken">
        /// A Betfair session token.
        /// </param>
        public BetfairApiClient(string appKey, string sessionToken)
        {
            this.appKey = appKey;
            this.sessionToken = sessionToken;

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            this.httpClient = new HttpClient(handler);
            this.SetHttpHeaders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetfairApiClient"/> class. 
        /// For use in unit tests where the HttpClient dependency needs to be injected.
        /// </summary>
        /// <param name="appKey">
        /// The app key.
        /// </param>
        /// <param name="sessionToken">
        /// The session token.
        /// </param>
        /// <param name="httpClient">
        /// The http betfairClient.
        /// </param>
        internal BetfairApiClient(string appKey, string sessionToken, HttpClient httpClient)
        {
            this.appKey = appKey;
            this.sessionToken = sessionToken;
            this.httpClient = httpClient;
            this.SetHttpHeaders();
        }

        /// <summary>
        /// Place orders.
        /// </summary>
        /// <param name="placeOrdersRequest">
        /// The place orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task{PlaceOrdersResponse}"/>.
        /// </returns>
        public async Task<PlaceOrdersResponse> PlaceOrders(PlaceOrdersRequest placeOrdersRequest)
        {
            return await this.SendRequestAsync<PlaceOrdersResponse>(placeOrdersRequest);
        }

        /// <summary>
        /// Cancel orders.
        /// </summary>
        /// <param name="cancelOrdersRequest">
        /// The cancel orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task{CancelOrdersResponse}"/>.
        /// </returns>
        public async Task<CancelOrdersResponse> CancelOrders(CancelOrdersRequest cancelOrdersRequest)
        {
            return await this.SendRequestAsync<CancelOrdersResponse>(cancelOrdersRequest);
        }

        /// <summary>
        /// Replace orders.
        /// </summary>
        /// <param name="replaceOrdersRequest">
        /// The replace orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ReplaceOrdersResponse> ReplaceOrders(ReplaceOrdersRequest replaceOrdersRequest)
        {
            return await this.SendRequestAsync<ReplaceOrdersResponse>(replaceOrdersRequest);
        }

        /// <summary>
        /// The send request async.
        /// </summary>
        /// <param name="requestObject">
        /// The request object.
        /// </param>
        /// <typeparam name="T">
        /// The response type
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<T> SendRequestAsync<T>(dynamic requestObject)
        {
            var serializedRequest = JsonConvert.SerializeObject(requestObject);

            var request = new HttpRequestMessage(HttpMethod.Post, "exchange/betting/json-rpc/v1")
            {
                Content = new StringContent(serializedRequest)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var response = await this.httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                try
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return stream.ReadAndDeserializeFromJson<T>();
                }
                catch
                {
                    return default;
                }
            }
        }

        /// <summary>
        /// The set http headers.
        /// </summary>
        private void SetHttpHeaders()
        {
            this.httpClient.BaseAddress = new Uri(BaseAddress);
            this.httpClient.Timeout = new TimeSpan(0, 0, 30);
            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("X-Application", this.appKey);
            this.httpClient.DefaultRequestHeaders.Add("X-Authentication", this.sessionToken);
            this.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }
    }
}
