using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Requests;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetCurrencyRates.Requests;

public class CurrencyRatesRequestTests
{
    [Fact]
    public void CanSerializeEmptyRequest()
    {
        var request = new CurrencyRatesRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{}");
    }

    [Fact]
    public void CanSerializeRequestWithFromCurrency()
    {
        var request = new CurrencyRatesRequest { FromCurrency = "USD" };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{\"fromCurrency\":\"USD\"}");
    }

    [Fact]
    public void CanSerializeRequestWithNullFromCurrency()
    {
        var request = new CurrencyRatesRequest { FromCurrency = null };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{}");
    }

    [Fact]
    public void CanDeserializeEmptyRequest()
    {
        const string json = "{}";

        var request = JsonSerializer.Deserialize(json, SerializerContext.Default.CurrencyRatesRequest);

        request.Should().NotBeNull();
        request!.FromCurrency.Should().BeNull();
    }

    [Fact]
    public void RequestShouldBeInternalClass()
    {
        var requestType = typeof(CurrencyRatesRequest);

        requestType.IsPublic.Should().BeFalse();
        requestType.IsNotPublic.Should().BeTrue();
    }

    [Fact]
    public void RequestShouldHaveParameterlessConstructor()
    {
        var requestType = typeof(CurrencyRatesRequest);
        var constructor = requestType.GetConstructor(Type.EmptyTypes);

        constructor.Should().NotBeNull();
        constructor!.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void RequestShouldHaveFromCurrencyProperty()
    {
        var requestType = typeof(CurrencyRatesRequest);
        var property = requestType.GetProperty("FromCurrency");

        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
        property.CanRead.Should().BeTrue();
        property.CanWrite.Should().BeTrue();
    }

    [Fact]
    public void FromCurrencyPropertyShouldBeNullableString()
    {
        var requestType = typeof(CurrencyRatesRequest);
        var property = requestType.GetProperty("FromCurrency");

        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));

        // Check if the property allows null values by checking the nullable reference type annotation
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(property);
        nullabilityInfo.WriteState.Should().Be(NullabilityState.Nullable);
    }

    [Fact]
    public void CanCreateMultipleInstances()
    {
        var request1 = new CurrencyRatesRequest();
        var request2 = new CurrencyRatesRequest();

        request1.Should().NotBeSameAs(request2);
        request1.Should().BeOfType<CurrencyRatesRequest>();
        request2.Should().BeOfType<CurrencyRatesRequest>();
    }

    [Fact]
    public void RequestShouldHaveCorrectNamespace()
    {
        var requestType = typeof(CurrencyRatesRequest);

        requestType.Namespace.Should().Be("Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Requests");
    }

    [Fact]
    public void RequestShouldHaveCorrectClassName()
    {
        var requestType = typeof(CurrencyRatesRequest);

        requestType.Name.Should().Be("CurrencyRatesRequest");
    }

    [Fact]
    public void RequestShouldBeRegisteredInSerializerContext()
    {
        var contextProperty = typeof(SerializerContext).GetProperty("CurrencyRatesRequest");

        contextProperty.Should().NotBeNull();
        contextProperty!.PropertyType.Should().Be(typeof(JsonTypeInfo<CurrencyRatesRequest>));
    }

    [Fact]
    public void FromCurrencyPropertyShouldHaveJsonPropertyNameAttribute()
    {
        var requestType = typeof(CurrencyRatesRequest);
        var property = requestType.GetProperty("FromCurrency");

        property.Should().NotBeNull();

        var jsonPropertyNameAttribute = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonPropertyNameAttribute.Should().NotBeNull();
        jsonPropertyNameAttribute!.Name.Should().Be("fromCurrency");
    }

    [Fact]
    public void SerializedJsonShouldUseCorrectPropertyNames()
    {
        var request = new CurrencyRatesRequest { FromCurrency = "USD" };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Contain("\"fromCurrency\":");
        json.Should().NotContain("\"FromCurrency\":");
    }

    [Fact]
    public void EmptyRequestShouldSerializeToEmptyObject()
    {
        var request = new CurrencyRatesRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{}");
        json.Should().NotContain(":");
        json.Should().NotContain("\"fromCurrency\"");
    }

    [Fact]
    public void RequestWithEmptyStringFromCurrencyShouldSerializeProperty()
    {
        var request = new CurrencyRatesRequest { FromCurrency = string.Empty };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{\"fromCurrency\":\"\"}");
    }

    [Fact]
    public void RequestWithWhitespaceFromCurrencyShouldSerializeProperty()
    {
        var request = new CurrencyRatesRequest { FromCurrency = " " };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{\"fromCurrency\":\" \"}");
    }
}
