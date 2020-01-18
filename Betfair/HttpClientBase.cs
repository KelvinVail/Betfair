namespace Betfair
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public abstract class HttpClientBase : IDisposable
    {
        private readonly Uri baseAddress;

        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private bool disposed;

        protected HttpClientBase(Uri baseAddress)
        {
            this.baseAddress = baseAddress;
            this.HttpClient = this.Configure(new HttpClient());
        }

        protected HttpClient HttpClient { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected HttpClientBase WithHttpClient(HttpClient httpClient)
        {
            if (httpClient is null) throw new ArgumentNullException(nameof(httpClient));
            this.HttpClient = this.Configure(httpClient);
            return this;
        }

        protected async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            this.disposables.Add(request);
            var response = await this.HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"{response.StatusCode}");
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing) this.DisposeManaged();

            this.disposed = true;
        }

        private void DisposeManaged()
        {
            this.disposables.ForEach(d => d.Dispose());
            ((IDisposable)this.HttpClient).Dispose();
        }

        private HttpClient Configure(HttpClient httpClient)
        {
            httpClient.BaseAddress = this.baseAddress;
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
