using Betfair.Api.Betting.Endpoints.ListMarketBook;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class HttpDeserializerTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly Uri _uri = new ("http://test.com/");
    private readonly HttpContent _content = new StringContent("content");
    private readonly BetfairHttpClient _httpClient;
    private readonly HttpDeserializer _client;
    private bool _disposedValue;

    public HttpDeserializerTests()
    {
        _httpClient = new BetfairHttpClient(_handler);
        _client = new HttpDeserializer(_httpClient);
    }

    [Fact]
    public async Task ResponsesShouldBeDeserialized()
    {
        var expectedResponse = new MarketBook[] { new () { Status = MarketStatus.Open } };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _client.PostAsync<MarketBook[]>(_uri, _content);

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostShouldThrowResponseBodyIfBadRequestIsReturned()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        var response = new BadRequestResponse();
        response.Detail.ApiNgException.ErrorCode = "INVALID_SESSION_INFORMATION";
        _handler.RespondsWithBody = response;

        var act = async () => { await _client.PostAsync<MarketBook>(_uri, _content); };

        var expectedMessage = JsonSerializer.Serialize(response, response.GetInternalContext());
        (await act.Should().ThrowAsync<HttpRequestException>())
            .WithMessage(expectedMessage)
            .And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

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
            _httpClient.Dispose();
            _handler.Dispose();
            _content.Dispose();
        }

        _disposedValue = true;
    }
}
