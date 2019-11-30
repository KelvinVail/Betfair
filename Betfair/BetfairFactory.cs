namespace Betfair
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class BetfairFactory
    {
        private readonly string appKey;

        private readonly string username;

        private readonly string password;

        private readonly Uri identityUri = 
            new Uri("https://identitysso-cert.betfair.com/");

        private HttpClient identityHttpClient;

        public BetfairFactory(string appKey, string username, string password)
        {
            this.appKey = appKey;
            this.username = username;
            this.password = password;
            this.identityHttpClient = new HttpClient();
            this.SetIdentityHeaders();
        }

        public Uri IdentityBaseAddress => this.identityHttpClient.BaseAddress;

        public int IdentityTimeout
        {
            get => (int)this.identityHttpClient.Timeout.TotalSeconds;
            set => this.identityHttpClient.Timeout = TimeSpan.FromSeconds(value);
        }

        public string IdentityAcceptMediaType => this.identityHttpClient.DefaultRequestHeaders.Accept.ToString();

        public BetfairFactory WithIdentityHttpClient(HttpClient httpClient)
        {
            this.identityHttpClient = httpClient;
            this.SetIdentityHeaders();
            return this;
        }

        public BetfairClient Build()
        {
            return new BetfairClient(this.identityHttpClient)
                       {
                           AppKey = this.appKey, Username = this.username, Password = this.password
                       };
        }

        private void SetIdentityHeaders()
        {
            this.identityHttpClient.BaseAddress = this.identityUri;
            this.identityHttpClient.Timeout = TimeSpan.FromSeconds(30);
            this.identityHttpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
