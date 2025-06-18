using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetCurrencyRates.Responses;

public class CurrencyRateTests
{
    [Fact]
    public void CanSerializeResponseWithAllProperties()
    {
        var response = new CurrencyRate
        {
            CurrencyCode = "USD",
            Rate = 1.25,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.CurrencyRate);

        json.Should().Contain("\"currencyCode\":\"USD\"");
        json.Should().Contain("\"rate\":1.25");
    }

    [Fact]
    public void CanDeserializeResponseWithAllProperties()
    {
        const string json = """
            {
                "currencyCode": "GBP",
                "rate": 0.85
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().Be("GBP");
        response.Rate.Should().Be(0.85);
    }

    [Fact]
    public void CanSerializeResponseWithNullCurrencyCode()
    {
        var response = new CurrencyRate
        {
            CurrencyCode = null,
            Rate = 1.0,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.CurrencyRate);

        json.Should().Contain("\"rate\":1");
        json.Should().NotContain("\"currencyCode\":");
    }

    [Fact]
    public void CanDeserializeResponseWithNullCurrencyCode()
    {
        const string json = """
            {
                "currencyCode": null,
                "rate": 1.5
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.Rate.Should().Be(1.5);
    }

    [Fact]
    public void CanDeserializeResponseWithMissingCurrencyCode()
    {
        const string json = """
            {
                "rate": 2.0
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.Rate.Should().Be(2.0);
    }

    [Fact]
    public void CanDeserializeResponseWithMissingRate()
    {
        const string json = """
            {
                "currencyCode": "EUR"
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().Be("EUR");
        response.Rate.Should().Be(0.0);
    }

    [Fact]
    public void CanSerializeAndDeserializeRoundTrip()
    {
        var originalResponse = new CurrencyRate
        {
            CurrencyCode = "JPY",
            Rate = 110.25,
        };

        var json = JsonSerializer.Serialize(originalResponse, SerializerContext.Default.CurrencyRate);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        deserializedResponse.Should().BeEquivalentTo(originalResponse);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(100.0)]
    [InlineData(1000.0)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.Epsilon)]
    [InlineData(-1.0)]
    public void CanHandleDifferentRateValues(double rate)
    {
        var response = new CurrencyRate
        {
            CurrencyCode = "USD",
            Rate = rate,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.CurrencyRate);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        deserializedResponse!.Rate.Should().Be(rate);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    [InlineData("USD")]
    [InlineData("GBP")]
    [InlineData("EUR")]
    [InlineData("JPY")]
    [InlineData("VeryLongCurrencyCodeThatShouldStillWork")]
    public void CanHandleDifferentCurrencyCodeValues(string currencyCode)
    {
        var response = new CurrencyRate
        {
            CurrencyCode = currencyCode,
            Rate = 1.0,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.CurrencyRate);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        deserializedResponse!.CurrencyCode.Should().Be(currencyCode);
    }

    [Fact]
    public void ResponseShouldBePublicClass()
    {
        var responseType = typeof(CurrencyRate);

        responseType.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void AllPropertiesShouldHaveInitAccessors()
    {
        var responseType = typeof(CurrencyRate);
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
        var responseType = typeof(CurrencyRate);

        responseType.Namespace.Should().Be("Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Responses");
    }

    [Fact]
    public void ResponseShouldHaveCorrectClassName()
    {
        var responseType = typeof(CurrencyRate);

        responseType.Name.Should().Be("CurrencyRate");
    }

    [Fact]
    public void ResponseShouldBeRegisteredInSerializerContext()
    {
        var contextProperty = typeof(SerializerContext).GetProperty("CurrencyRate");

        contextProperty.Should().NotBeNull();
        contextProperty!.PropertyType.Should().Be(typeof(JsonTypeInfo<CurrencyRate>));
    }

    [Fact]
    public void ResponseArrayShouldBeRegisteredInSerializerContext()
    {
        var contextProperty = typeof(SerializerContext).GetProperty("CurrencyRateArray");

        contextProperty.Should().NotBeNull();
        contextProperty!.PropertyType.Should().Be(typeof(JsonTypeInfo<CurrencyRate[]>));
    }

    [Fact]
    public void CanDeserializeEmptyJson()
    {
        const string json = "{}";

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().BeNull();
        response.Rate.Should().Be(0.0);
    }

    [Fact]
    public void CanDeserializeArrayOfCurrencyRates()
    {
        const string json = """
            [
                {
                    "currencyCode": "USD",
                    "rate": 1.25
                },
                {
                    "currencyCode": "EUR",
                    "rate": 0.92
                }
            ]
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRateArray);

        response.Should().NotBeNull();
        response.Should().HaveCount(2);
        response![0].CurrencyCode.Should().Be("USD");
        response[0].Rate.Should().Be(1.25);
        response[1].CurrencyCode.Should().Be("EUR");
        response[1].Rate.Should().Be(0.92);
    }

    [Fact]
    public void CanSerializeArrayOfCurrencyRates()
    {
        var rates = new[]
        {
            new CurrencyRate { CurrencyCode = "GBP", Rate = 0.85 },
            new CurrencyRate { CurrencyCode = "JPY", Rate = 110.0 },
        };

        var json = JsonSerializer.Serialize(rates, SerializerContext.Default.CurrencyRateArray);

        json.Should().Contain("\"currencyCode\":\"GBP\"");
        json.Should().Contain("\"rate\":0.85");
        json.Should().Contain("\"currencyCode\":\"JPY\"");
        json.Should().Contain("\"rate\":110");
    }

    [Fact]
    public void CanHandleEmptyArray()
    {
        var rates = Array.Empty<CurrencyRate>();

        var json = JsonSerializer.Serialize(rates, SerializerContext.Default.CurrencyRateArray);
        var deserializedRates = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRateArray);

        deserializedRates.Should().NotBeNull();
        deserializedRates.Should().BeEmpty();
    }

    [Fact]
    public void CurrencyCodePropertyShouldBeNullableString()
    {
        var responseType = typeof(CurrencyRate);
        var property = responseType.GetProperty("CurrencyCode");

        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));

        // Check if the property allows null values by checking the nullable reference type annotation
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(property);
        nullabilityInfo.WriteState.Should().Be(NullabilityState.Nullable);
    }

    [Fact]
    public void RatePropertyShouldBeDouble()
    {
        var responseType = typeof(CurrencyRate);
        var property = responseType.GetProperty("Rate");

        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(double));
    }

    [Fact]
    public void CanHandleSpecialDoubleValues()
    {
        // Test only values that can be serialized to JSON
        var testCases = new[]
        {
            double.Epsilon,
            -double.Epsilon,
            1e-10,
            1e10,
        };

        foreach (var testValue in testCases)
        {
            var response = new CurrencyRate
            {
                CurrencyCode = "TEST",
                Rate = testValue,
            };

            var json = JsonSerializer.Serialize(response, SerializerContext.Default.CurrencyRate);
            var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

            deserializedResponse!.Rate.Should().Be(testValue);
        }
    }

    [Fact]
    public void CanHandleExtraPropertiesInJson()
    {
        const string json = """
            {
                "currencyCode": "USD",
                "rate": 1.25,
                "extraProperty": "value",
                "anotherProperty": 123
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRate);

        response.Should().NotBeNull();
        response!.CurrencyCode.Should().Be("USD");
        response.Rate.Should().Be(1.25);
    }
}
