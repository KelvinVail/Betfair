using Betfair.Api.Accounts.Exceptions;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;
using Betfair.Api.Betting.Exceptions;
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
    public async Task PostShouldThrowSpecificExceptionForBetfairErrors()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        var response = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "ANGX-0003",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "ie2-ang25b-prd-05260933-002978bb9b",
                    ErrorCode = "INVALID_SESSION_INFORMATION",
                    ErrorDetails = string.Empty,
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = response;

        var act = async () => { await _client.PostAsync<MarketBook>(_uri, _content); };

        var exception = await act.Should().ThrowAsync<BettingInvalidSessionInformationException>();
        exception.Which.Message.Should().StartWith("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");
        exception.Which.Message.Should().Contain("ANGX-0003");
        exception.Which.Message.Should().Contain("INVALID_SESSION_INFORMATION");
        exception.Which.RequestUUID.Should().Be("ie2-ang25b-prd-05260933-002978bb9b");
        exception.Which.ErrorDetails.Should().Be(string.Empty);
    }

    [Fact]
    public async Task PostShouldFallbackToHttpRequestExceptionForNonBetfairErrors()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.InternalServerError;
        const string errorContent = "Internal Server Error";
        _handler.RespondsWithStringBody = errorContent;

        var act = async () => { await _client.PostAsync<MarketBook>(_uri, _content); };

        var exception = await act.Should().ThrowAsync<HttpRequestException>();
        exception.Which.Message.Should().Be(errorContent);
    }

    [Fact]
    public async Task PostShouldThrowAccountExceptionForAccountApiErrors()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        var response = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "ANGX-0003",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "account-request-uuid",
                    ErrorCode = "INVALID_SESSION_INFORMATION",
                    ErrorDetails = "Session expired",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = response;

        var accountUri = new Uri("https://api.betfair.com/exchange/account/rest/v1.0/getAccountFunds/");
        var act = async () => { await _client.PostAsync<MarketBook>(accountUri, _content); };

        var exception = await act.Should().ThrowAsync<InvalidSessionInformationException>();
        exception.Which.Message.Should().StartWith("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");
        exception.Which.Message.Should().Contain("ANGX-0003");
        exception.Which.Message.Should().Contain("INVALID_SESSION_INFORMATION");
        exception.Which.RequestUUID.Should().Be("account-request-uuid");
        exception.Which.ErrorDetails.Should().Be("Session expired");
    }

    [Fact]
    public void BetfairExceptionFactoryPreservesOriginalMessageAndAppendsErrorDetails()
    {
        // Test that the original descriptive message is preserved and Betfair details are appended
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "ANGX-0003",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "test-uuid",
                    ErrorCode = "INVALID_SESSION_INFORMATION",
                    ErrorDetails = "Additional details",
                },
                ExceptionName = "APINGException",
            },
        };

        var exception = (BettingInvalidSessionInformationException)Betfair.Core.Client.BetfairExceptionFactory.CreateBettingException(errorResponse, HttpStatusCode.BadRequest);

        // Should start with the original descriptive message
        exception.Message.Should().StartWith("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");

        // Should contain the Betfair error details
        exception.Message.Should().Contain("Betfair error: ANGX-0003");
        exception.Message.Should().Contain("Error code: INVALID_SESSION_INFORMATION");

        // Should have the properties set
        exception.RequestUUID.Should().Be("test-uuid");
        exception.ErrorDetails.Should().Be("Additional details");
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
