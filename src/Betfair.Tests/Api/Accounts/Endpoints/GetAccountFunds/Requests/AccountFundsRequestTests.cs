using System.Text.Json.Serialization;
using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Requests;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountFunds.Requests;

public class AccountFundsRequestTests
{
    [Fact]
    public void CanSerializeRequestWithDefaultWallet()
    {
        var request = new AccountFundsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountFundsRequest);

        json.Should().Contain("\"wallet\":\"UK\"");
    }

    [Fact]
    public void RequestShouldBeInternalClass()
    {
        var requestType = typeof(AccountFundsRequest);

        requestType.IsPublic.Should().BeFalse();
        requestType.IsNotPublic.Should().BeTrue();
    }

    [Fact]
    public void WalletPropertyShouldBeReadOnly()
    {
        var walletProperty = typeof(AccountFundsRequest).GetProperty(nameof(AccountFundsRequest.Wallet));

        walletProperty.Should().NotBeNull();
        walletProperty!.CanRead.Should().BeTrue();
        walletProperty.CanWrite.Should().BeFalse();
    }

    [Fact]
    public void WalletPropertyShouldHaveCorrectJsonPropertyName()
    {
        var walletProperty = typeof(AccountFundsRequest).GetProperty(nameof(AccountFundsRequest.Wallet));
        var jsonPropertyNameAttribute = walletProperty!.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
            .Cast<JsonPropertyNameAttribute>()
            .FirstOrDefault();

        jsonPropertyNameAttribute.Should().NotBeNull();
        jsonPropertyNameAttribute!.Name.Should().Be("wallet");
    }

    [Fact]
    public void DefaultWalletShouldBeUK()
    {
        var request = new AccountFundsRequest();

        request.Wallet.Should().Be("UK");
    }

    [Fact]
    public void SerializedJsonShouldBeMinimal()
    {
        var request = new AccountFundsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountFundsRequest);

        // Should only contain the wallet property
        json.Should().Be("{\"wallet\":\"UK\"}");
    }

    [Fact]
    public void SerializationShouldBeConsistent()
    {
        var request1 = new AccountFundsRequest();
        var request2 = new AccountFundsRequest();

        var json1 = JsonSerializer.Serialize(request1, SerializerContext.Default.AccountFundsRequest);
        var json2 = JsonSerializer.Serialize(request2, SerializerContext.Default.AccountFundsRequest);

        json1.Should().Be(json2);
        json1.Should().Be("{\"wallet\":\"UK\"}");
    }

    [Fact]
    public void SerializationShouldIncludeWalletProperty()
    {
        var request = new AccountFundsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountFundsRequest);

        json.Should().Contain("\"wallet\":");
        json.Should().Contain("\"UK\"");
    }
}
