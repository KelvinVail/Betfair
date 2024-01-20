using System.Security.Cryptography.X509Certificates;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class BetfairHttpClientTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly BetfairHttpClient _client = new ((X509Certificate2?)null);
    private readonly Uri _uri = new ("http://test.com/");
    private bool _disposedValue;

    [Fact]
    public void TimeoutIsSetToThirtySeconds() =>
        _client.Timeout.Should().Be(TimeSpan.FromSeconds(30));

    [Fact]
    public void AcceptsApplicationJson() =>
        _client.DefaultRequestHeaders.Accept.Should().Contain(
            new MediaTypeWithQualityHeaderValue("application/json"));

    [Fact]
    public void ConnectionIsKeptAlive() =>
        _client.DefaultRequestHeaders.Connection.Should().Contain("keep-alive");

    [Fact]
    public void AcceptGzipEncoding() =>
        _client.DefaultRequestHeaders.AcceptEncoding.Should().Contain(
            new StringWithQualityHeaderValue("gzip"));

    //[Fact]
    //public async Task ReturnErrorIfPostIsNotOk()
    //{
    //    _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;

    //    var ex = await _client.Invoking(x => x.Post<object>(_uri))
    //        .Should().ThrowAsync<HttpRequestException>();

    //    ex.And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //}

    //[Fact]
    //public async Task GetTokenCallsBetfairLoginOnFirstExecution()
    //{
    //    await _client.GetToken();

    //    _handler.UriCalled.Should().Be(new Uri("https://identitysso.betfair.com/api/login"));
    //}

    //[Fact]
    //public async Task GetTokenOnlyCallsBetfairLoginOnFirstCall()
    //{
    //    await _client.GetToken();
    //    await _client.GetToken();

    //    _handler.TimesUriCalled
    //        .Should().ContainKey(new Uri("https://identitysso.betfair.com/api/login"))
    //        .WhoseValue.Should().Be(1);
    //}

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing)
        {
            _client.Dispose();
            _handler.Dispose();
        }

        _disposedValue = true;
    }
}
