using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Betfair
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Session : IDisposable
    {
        private readonly string appKey;

        private readonly string username;

        private readonly string password;

        private readonly HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Post, "api/login");

        private HttpClient client = new HttpClient();

        public Session(string appKey, string username, string password)
        {
            if (string.IsNullOrEmpty(appKey)) throw new NullReferenceException($"{nameof(appKey)} not set.");
            if (string.IsNullOrEmpty(username)) throw new NullReferenceException($"{nameof(username)} not set.");
            if (string.IsNullOrEmpty(password)) throw new NullReferenceException($"{nameof(password)} not set.");

            this.appKey = appKey;
            this.username = username;
            this.password = password;
            this.ConfigureHttpClient();
        }

        public Session WithHttpClient(HttpClient httpClient)
        {
            this.client = httpClient ?? throw new NullReferenceException($"{nameof(httpClient)} is null.");
            this.ConfigureHttpClient();
            return this;
        }

        public async Task LoginAsync()
        {
            this.apiRequest.Headers.Add("X-Application", this.appKey);
            this.apiRequest.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.username }, { "password", this.password } });
            using (var response = await this.client.SendAsync(this.apiRequest))
            {
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.client.Dispose();
            this.apiRequest.Dispose();
        }

        private void ConfigureHttpClient()
        {
            this.client.BaseAddress = new Uri("https://identitysso.betfair.com");
            this.client.Timeout = TimeSpan.FromSeconds(30);
            this.client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
