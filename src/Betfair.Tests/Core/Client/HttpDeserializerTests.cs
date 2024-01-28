using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

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
        var expectedResponse = new Dictionary<string, string> { { "Key", "Value" } };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _client.PostAsync<Dictionary<string, string>>(_uri, _content);

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostShouldThrowIfBadRequestIsReturned()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        _handler.RespondsWithBody = new BadRequestResponse("INVALID_SESSION_INFORMATION");

        var act = async () => { await _client.PostAsync<object>(_uri, _content); };

        (await act.Should().ThrowAsync<HttpRequestException>())
            .WithMessage("INVALID_SESSION_INFORMATION")
            .And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostShouldThrowAGenericMessageIsErrorCodeNotFound()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        _handler.RespondsWithBody = new { Key = "Value" };

        var act = async () => { await _client.PostAsync<object>(_uri, _content); };

        (await act.Should().ThrowAsync<HttpRequestException>())
            .WithMessage("An HttpRequestException Occurred.")
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

    private sealed class BadRequestResponse
    {
        public BadRequestResponse(string errorCode) =>
            Detail.ApiNgException.ErrorCode = errorCode;

        public Detail Detail { get; } = new ();
    }

    private sealed class Detail
    {
        public ApiNgException ApiNgException { get; } = new ();
    }

    private sealed class ApiNgException
    {
        public string? ErrorCode { get; set; }
    }
}
