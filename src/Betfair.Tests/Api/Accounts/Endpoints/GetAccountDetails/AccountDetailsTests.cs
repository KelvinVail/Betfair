using System.Net;
using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses;
using Betfair.Api.Accounts.Exceptions;
using Betfair.Core.Client;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountDetails;

public class AccountDetailsTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _httpClient;
    private readonly HttpAdapter _adapter;
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public AccountDetailsTests()
    {
        _httpClient = new BetfairHttpClient(_handler);
        var deserializer = new HttpDeserializer(_httpClient);
        _adapter = new HttpAdapter(deserializer);
        _api = new BetfairApiClient(_adapter);
        _handler.RespondsWithBody = new AccountDetailsResponse();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.AccountDetails();

        _handler.UriCalled.Should().Be(
            new Uri("https://api.betfair.com/exchange/account/rest/v1.0/getAccountDetails/"));
    }

    [Fact]
    public async Task UsesPostMethod()
    {
        await _api.AccountDetails();

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.AccountDetails();

        _handler.StringContentSent.Should().Be("{}");
    }

    [Fact]
    public async Task RequestBodyShouldBeEmptyJson()
    {
        await _api.AccountDetails();

        var json = _handler.StringContentSent;
        json.Should().Be("{}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = "GBP",
            FirstName = "John",
            LastName = "Doe",
            LocaleCode = "en_GB",
            Region = "GBR",
            Timezone = "GMT",
            DiscountRate = 0.05,
            PointsBalance = 100,
            CountryCode = "GB",
        };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithAllNullPropertiesShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = null,
            FirstName = null,
            LastName = null,
            LocaleCode = null,
            Region = null,
            Timezone = null,
            DiscountRate = 0.0,
            PointsBalance = 0,
            CountryCode = null,
        };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithMinimalDataShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            DiscountRate = 0.0,
            PointsBalance = 0,
        };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithMaximumValuesShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = "USD",
            FirstName = "VeryLongFirstNameThatCouldPotentiallyBeUsedInSomeCountries",
            LastName = "VeryLongLastNameThatCouldPotentiallyBeUsedInSomeCountries",
            LocaleCode = "en_US",
            Region = "USA",
            Timezone = "America/New_York",
            DiscountRate = 1.0,
            PointsBalance = int.MaxValue,
            CountryCode = "US",
        };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithNegativeValuesShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = "EUR",
            FirstName = "Test",
            LastName = "User",
            LocaleCode = "de_DE",
            Region = "DEU",
            Timezone = "Europe/Berlin",
            DiscountRate = -0.1,
            PointsBalance = -100,
            CountryCode = "DE",
        };
        _handler.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        await _api.AccountDetails(token);

        // Verify the call was made (no exception thrown)
        _handler.UriCalled.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldThrowInvalidSessionInformationExceptionForInvalidSession()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "AANGX-0002",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "account-details-uuid",
                    ErrorCode = "INVALID_SESSION_INFORMATION",
                    ErrorDetails = "Session token expired",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<InvalidSessionInformationException>();
        exception.Which.Message.Should().StartWith("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");
        exception.Which.Message.Should().Contain("AANGX-0002");
        exception.Which.Message.Should().Contain("INVALID_SESSION_INFORMATION");
        exception.Which.RequestUUID.Should().Be("account-details-uuid");
        exception.Which.ErrorDetails.Should().Be("Session token expired");
    }

    [Fact]
    public async Task ShouldThrowInvalidAppKeyExceptionForInvalidAppKey()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "AANGX-0004",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "invalid-app-key-uuid",
                    ErrorCode = "INVALID_APP_KEY",
                    ErrorDetails = "Application key is invalid",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<InvalidAppKeyException>();
        exception.Which.Message.Should().StartWith("The application key passed is invalid");
        exception.Which.Message.Should().Contain("AANGX-0004");
        exception.Which.Message.Should().Contain("INVALID_APP_KEY");
        exception.Which.RequestUUID.Should().Be("invalid-app-key-uuid");
        exception.Which.ErrorDetails.Should().Be("Application key is invalid");
    }

    [Fact]
    public async Task ShouldThrowUnexpectedErrorExceptionForUnexpectedError()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.InternalServerError;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Server",
            FaultString = "AANGX-0003",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "unexpected-error-uuid",
                    ErrorCode = "UNEXPECTED_ERROR",
                    ErrorDetails = "Internal server error occurred",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<UnexpectedErrorException>();
        exception.Which.Message.Should().StartWith("An unexpected internal error occurred that prevented successful request processing.");
        exception.Which.Message.Should().Contain("AANGX-0003");
        exception.Which.Message.Should().Contain("UNEXPECTED_ERROR");
        exception.Which.RequestUUID.Should().Be("unexpected-error-uuid");
        exception.Which.ErrorDetails.Should().Be("Internal server error occurred");
    }

    [Fact]
    public async Task ShouldThrowServiceBusyExceptionForServiceBusy()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.ServiceUnavailable;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Server",
            FaultString = "AANGX-0007",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "service-busy-uuid",
                    ErrorCode = "SERVICE_BUSY",
                    ErrorDetails = "Service is currently busy",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<ServiceBusyException>();
        exception.Which.Message.Should().StartWith("The service is currently too busy to service this request.");
        exception.Which.Message.Should().Contain("AANGX-0007");
        exception.Which.Message.Should().Contain("SERVICE_BUSY");
        exception.Which.RequestUUID.Should().Be("service-busy-uuid");
        exception.Which.ErrorDetails.Should().Be("Service is currently busy");
    }

    [Fact]
    public async Task ShouldThrowTimeoutErrorExceptionForTimeout()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.RequestTimeout;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Server",
            FaultString = "AANGX-0008",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "timeout-uuid",
                    ErrorCode = "TIMEOUT_ERROR",
                    ErrorDetails = "Request timed out",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<TimeoutErrorException>();
        exception.Which.Message.Should().StartWith("The internal call to downstream service timed out.");
        exception.Which.Message.Should().Contain("AANGX-0008");
        exception.Which.Message.Should().Contain("TIMEOUT_ERROR");
        exception.Which.RequestUUID.Should().Be("timeout-uuid");
        exception.Which.ErrorDetails.Should().Be("Request timed out");
    }

    [Fact]
    public async Task ShouldThrowTooManyRequestsExceptionForRateLimit()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.TooManyRequests;
        var errorResponse = new BetfairErrorResponse
        {
            FaultCode = "Client",
            FaultString = "AANGX-0010",
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    RequestUUID = "rate-limit-uuid",
                    ErrorCode = "TOO_MANY_REQUESTS",
                    ErrorDetails = "Rate limit exceeded",
                },
                ExceptionName = "APINGException",
            },
        };
        _handler.RespondsWithBody = errorResponse;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<TooManyRequestsException>();
        exception.Which.Message.Should().StartWith("Too many requests. For more details relating to this error please see FAQ's.");
        exception.Which.Message.Should().Contain("AANGX-0010");
        exception.Which.Message.Should().Contain("TOO_MANY_REQUESTS");
        exception.Which.RequestUUID.Should().Be("rate-limit-uuid");
        exception.Which.ErrorDetails.Should().Be("Rate limit exceeded");
    }

    [Fact]
    public async Task ShouldFallbackToHttpRequestExceptionForNonBetfairErrors()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.InternalServerError;
        const string errorContent = "Internal Server Error - Not a Betfair error";
        _handler.RespondsWithBody = errorContent;

        var act = async () => await _api.AccountDetails();

        var exception = await act.Should().ThrowAsync<HttpRequestException>();
        exception.Which.Message.Should().Be(errorContent);
    }

    [Fact]
    public async Task ShouldHandleCancellationTokenCancellation()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await _api.AccountDetails(cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ShouldSetCorrectContentTypeHeader()
    {
        await _api.AccountDetails();

        _handler.ContentHeadersSent.Should().NotBeNull();
        _handler.ContentHeadersSent!.ContentType.Should().NotBeNull();
        _handler.ContentHeadersSent.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task ShouldHandleEmptyResponseBody()
    {
        _handler.RespondsWithBody = string.Empty;

        var act = async () => await _api.AccountDetails();

        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task ShouldHandleInvalidJsonResponse()
    {
        _handler.RespondsWithBody = "invalid json content";

        var act = async () => await _api.AccountDetails();

        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task ShouldHandleNullResponse()
    {
        _handler.RespondsWithBody = "null";

        var response = await _api.AccountDetails();

        response.Should().BeNull();
    }

    [Fact]
    public async Task ShouldHandleResponseWithExtraProperties()
    {
        const string jsonWithExtraProperties = """
            {
                "currencyCode": "GBP",
                "firstName": "John",
                "lastName": "Doe",
                "localeCode": "en_GB",
                "region": "GBR",
                "timezone": "GMT",
                "discountRate": 0.05,
                "pointsBalance": 100,
                "countryCode": "GB",
                "extraProperty1": "should be ignored",
                "extraProperty2": 12345,
                "extraProperty3": true
            }
            """;
        _handler.RespondsWithBody = jsonWithExtraProperties;

        var response = await _api.AccountDetails();

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().Be("GBP");
        response.FirstName.Should().Be("John");
        response.LastName.Should().Be("Doe");
        response.LocaleCode.Should().Be("en_GB");
        response.Region.Should().Be("GBR");
        response.Timezone.Should().Be("GMT");
        response.DiscountRate.Should().Be(0.05);
        response.PointsBalance.Should().Be(100);
        response.CountryCode.Should().Be("GB");
    }

    [Fact]
    public void ShouldBeVirtualMethodForMocking()
    {
        var apiType = typeof(BetfairApiClient);
        var method = apiType.GetMethod("AccountDetails");

        method.Should().NotBeNull();
        method!.IsVirtual.Should().BeTrue("AccountDetails method should be virtual to allow mocking");
    }

    [Fact]
    public async Task MultipleCallsShouldWorkCorrectly()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = "GBP",
            FirstName = "John",
            LastName = "Doe",
            DiscountRate = 0.05,
            PointsBalance = 100,
        };
        _handler.RespondsWithBody = expectedResponse;

        var response1 = await _api.AccountDetails();
        var response2 = await _api.AccountDetails();

        response1.Should().BeEquivalentTo(expectedResponse);
        response2.Should().BeEquivalentTo(expectedResponse);
        _handler.TimesUriCalled[new Uri("https://api.betfair.com/exchange/account/rest/v1.0/getAccountDetails/")].Should().Be(2);
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
            _handler.Dispose();
            _httpClient.Dispose();
            _api.Dispose();
        }

        _disposedValue = true;
    }
}
