using System.Text.Json.Serialization.Metadata;
using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountDetails.Responses;

public class AccountDetailsResponseTests
{
    [Fact]
    public void CanSerializeResponseWithAllProperties()
    {
        var response = new AccountDetailsResponse
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

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountDetailsResponse);

        json.Should().Contain("\"currencyCode\":\"GBP\"");
        json.Should().Contain("\"firstName\":\"John\"");
        json.Should().Contain("\"lastName\":\"Doe\"");
        json.Should().Contain("\"localeCode\":\"en_GB\"");
        json.Should().Contain("\"region\":\"GBR\"");
        json.Should().Contain("\"timezone\":\"GMT\"");
        json.Should().Contain("\"discountRate\":0.05");
        json.Should().Contain("\"pointsBalance\":100");
        json.Should().Contain("\"countryCode\":\"GB\"");
    }

    [Fact]
    public void CanDeserializeResponseWithAllProperties()
    {
        const string json = """
            {
                "currencyCode": "USD",
                "firstName": "Jane",
                "lastName": "Smith",
                "localeCode": "en_US",
                "region": "USA",
                "timezone": "EST",
                "discountRate": 0.1,
                "pointsBalance": 250,
                "countryCode": "US"
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().Be("USD");
        response.FirstName.Should().Be("Jane");
        response.LastName.Should().Be("Smith");
        response.LocaleCode.Should().Be("en_US");
        response.Region.Should().Be("USA");
        response.Timezone.Should().Be("EST");
        response.DiscountRate.Should().Be(0.1);
        response.PointsBalance.Should().Be(250);
        response.CountryCode.Should().Be("US");
    }

    [Fact]
    public void CanSerializeResponseWithNullProperties()
    {
        var response = new AccountDetailsResponse
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

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountDetailsResponse);

        json.Should().Contain("\"discountRate\":0");
        json.Should().Contain("\"pointsBalance\":0");

        // Null properties are not serialized by default in System.Text.Json
        json.Should().NotContain("\"currencyCode\":");
        json.Should().NotContain("\"firstName\":");
        json.Should().NotContain("\"lastName\":");
        json.Should().NotContain("\"localeCode\":");
        json.Should().NotContain("\"region\":");
        json.Should().NotContain("\"timezone\":");
        json.Should().NotContain("\"countryCode\":");
    }

    [Fact]
    public void CanDeserializeResponseWithNullProperties()
    {
        const string json = """
            {
                "currencyCode": null,
                "firstName": null,
                "lastName": null,
                "localeCode": null,
                "region": null,
                "timezone": null,
                "discountRate": 0.0,
                "pointsBalance": 0,
                "countryCode": null
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.FirstName.Should().BeNull();
        response.LastName.Should().BeNull();
        response.LocaleCode.Should().BeNull();
        response.Region.Should().BeNull();
        response.Timezone.Should().BeNull();
        response.DiscountRate.Should().Be(0.0);
        response.PointsBalance.Should().Be(0);
        response.CountryCode.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeResponseWithMissingProperties()
    {
        const string json = """
            {
                "discountRate": 0.02,
                "pointsBalance": 50
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.FirstName.Should().BeNull();
        response.LastName.Should().BeNull();
        response.LocaleCode.Should().BeNull();
        response.Region.Should().BeNull();
        response.Timezone.Should().BeNull();
        response.DiscountRate.Should().Be(0.02);
        response.PointsBalance.Should().Be(50);
        response.CountryCode.Should().BeNull();
    }

    [Fact]
    public void CanSerializeAndDeserializeRoundTrip()
    {
        var originalResponse = new AccountDetailsResponse
        {
            CurrencyCode = "EUR",
            FirstName = "Pierre",
            LastName = "Dupont",
            LocaleCode = "fr_FR",
            Region = "FRA",
            Timezone = "CET",
            DiscountRate = 0.03,
            PointsBalance = 75,
            CountryCode = "FR",
        };

        var json = JsonSerializer.Serialize(originalResponse, SerializerContext.Default.AccountDetailsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        deserializedResponse.Should().BeEquivalentTo(originalResponse);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public void CanHandleDifferentDiscountRateValues(double discountRate)
    {
        var response = new AccountDetailsResponse
        {
            DiscountRate = discountRate,
            PointsBalance = 100,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountDetailsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        deserializedResponse!.DiscountRate.Should().Be(discountRate);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1000)]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void CanHandleDifferentPointsBalanceValues(int pointsBalance)
    {
        var response = new AccountDetailsResponse
        {
            DiscountRate = 0.05,
            PointsBalance = pointsBalance,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountDetailsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        deserializedResponse!.PointsBalance.Should().Be(pointsBalance);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    [InlineData("VeryLongCurrencyCodeThatShouldStillWork")]
    public void CanHandleDifferentCurrencyCodeValues(string currencyCode)
    {
        var response = new AccountDetailsResponse
        {
            CurrencyCode = currencyCode,
            DiscountRate = 0.05,
            PointsBalance = 100,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountDetailsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        deserializedResponse!.CurrencyCode.Should().Be(currencyCode);
    }

    [Fact]
    public void ResponseShouldBePublicClass()
    {
        var responseType = typeof(AccountDetailsResponse);

        responseType.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void AllPropertiesShouldHaveInitAccessors()
    {
        var responseType = typeof(AccountDetailsResponse);
        var properties = responseType.GetProperties();

        foreach (var property in properties)
        {
            property.CanRead.Should().BeTrue($"Property {property.Name} should be readable");
            property.SetMethod.Should().NotBeNull($"Property {property.Name} should have a setter");

            // Check if it's an init-only setter by checking the return type modifiers
            var setMethod = property.SetMethod!;
            var returnParameter = setMethod.ReturnParameter;
            var requiredModifiers = returnParameter.GetRequiredCustomModifiers();

            // Init-only setters have IsExternalInit modifier
            var hasInitModifier = requiredModifiers.Any(t => t.Name == "IsExternalInit");
            hasInitModifier.Should().BeTrue($"Property {property.Name} should have init accessor");
        }
    }

    [Fact]
    public void ResponseShouldHaveCorrectNamespace()
    {
        var responseType = typeof(AccountDetailsResponse);

        responseType.Namespace.Should().Be("Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses");
    }

    [Fact]
    public void ResponseShouldBeRegisteredInSerializerContext()
    {
        var contextProperty = typeof(SerializerContext).GetProperty("AccountDetailsResponse");

        contextProperty.Should().NotBeNull();
        contextProperty!.PropertyType.Should().Be(typeof(JsonTypeInfo<AccountDetailsResponse>));
    }

    [Fact]
    public void CanDeserializeEmptyJson()
    {
        const string json = "{}";

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsResponse);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.FirstName.Should().BeNull();
        response.LastName.Should().BeNull();
        response.LocaleCode.Should().BeNull();
        response.Region.Should().BeNull();
        response.Timezone.Should().BeNull();
        response.DiscountRate.Should().Be(0.0);
        response.PointsBalance.Should().Be(0);
        response.CountryCode.Should().BeNull();
    }
}
