using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Requests;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountDetails.Requests;

public class AccountDetailsRequestTests
{
    [Fact]
    public void CanSerializeEmptyRequest()
    {
        var request = new AccountDetailsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountDetailsRequest);

        json.Should().Be("{}");
    }

    [Fact]
    public void CanDeserializeEmptyRequest()
    {
        const string json = "{}";

        var request = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsRequest);

        request.Should().NotBeNull();
    }

    [Fact]
    public void CanDeserializeNullJson()
    {
        const string json = "null";

        var request = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsRequest);

        request.Should().BeNull();
    }

    [Fact]
    public void CanSerializeAndDeserializeRoundTrip()
    {
        var originalRequest = new AccountDetailsRequest();

        var json = JsonSerializer.Serialize(originalRequest, SerializerContext.Default.AccountDetailsRequest);
        var deserializedRequest = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountDetailsRequest);

        deserializedRequest.Should().NotBeNull();

        // Since AccountDetailsRequest is an empty class, we just verify the types match
        deserializedRequest.Should().BeOfType<AccountDetailsRequest>();
    }

    [Fact]
    public void SerializedJsonShouldNotContainAnyProperties()
    {
        var request = new AccountDetailsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountDetailsRequest);

        json.Should().NotContain(":");
        json.Should().NotContain("\"");
        json.Should().Be("{}");
    }

    [Fact]
    public void RequestShouldBeInternalClass()
    {
        var requestType = typeof(AccountDetailsRequest);

        requestType.IsPublic.Should().BeFalse();
        requestType.IsNotPublic.Should().BeTrue();
    }

    [Fact]
    public void RequestShouldHaveParameterlessConstructor()
    {
        var requestType = typeof(AccountDetailsRequest);
        var constructor = requestType.GetConstructor(Type.EmptyTypes);

        constructor.Should().NotBeNull();

        // The constructor should be internal (not public) since the class is internal
        constructor!.IsPublic.Should().BeTrue(); // Actually it's public because the class is internal
    }

    [Fact]
    public void RequestShouldNotHaveAnyPublicProperties()
    {
        var requestType = typeof(AccountDetailsRequest);
        var publicProperties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        publicProperties.Should().BeEmpty();
    }

    [Fact]
    public void RequestShouldNotHaveAnyFields()
    {
        var requestType = typeof(AccountDetailsRequest);
        var fields = requestType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        fields.Should().BeEmpty();
    }

    [Fact]
    public void CanCreateMultipleInstances()
    {
        var request1 = new AccountDetailsRequest();
        var request2 = new AccountDetailsRequest();

        request1.Should().NotBeSameAs(request2);

        // Since AccountDetailsRequest is an empty class, we just verify the types match
        request1.Should().BeOfType<AccountDetailsRequest>();
        request2.Should().BeOfType<AccountDetailsRequest>();
    }

    [Fact]
    public void RequestShouldBeSerializableToSameJsonRegardlessOfInstance()
    {
        var request1 = new AccountDetailsRequest();
        var request2 = new AccountDetailsRequest();

        var json1 = JsonSerializer.Serialize(request1, SerializerContext.Default.AccountDetailsRequest);
        var json2 = JsonSerializer.Serialize(request2, SerializerContext.Default.AccountDetailsRequest);

        json1.Should().Be(json2);
        json1.Should().Be("{}");
    }

    [Fact]
    public void RequestShouldHaveCorrectNamespace()
    {
        var requestType = typeof(AccountDetailsRequest);

        requestType.Namespace.Should().Be("Betfair.Api.Accounts.Endpoints.GetAccountDetails.Requests");
    }

    [Fact]
    public void RequestShouldHaveCorrectClassName()
    {
        var requestType = typeof(AccountDetailsRequest);

        requestType.Name.Should().Be("AccountDetailsRequest");
    }

    [Fact]
    public void RequestShouldBeRegisteredInSerializerContext()
    {
        var contextProperty = typeof(SerializerContext).GetProperty("AccountDetailsRequest");

        contextProperty.Should().NotBeNull();
        contextProperty!.PropertyType.Should().Be(typeof(JsonTypeInfo<AccountDetailsRequest>));
    }
}
