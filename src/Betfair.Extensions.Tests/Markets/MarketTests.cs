using Betfair.Core.Login;
using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class MarketTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private Action<Market> _onUpdate = _ => { };

    [Fact]
    public void CredentialMustNotBeNull()
    {
        var result = Market.Create(null!, "1.1", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Credentials must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeNull()
    {
        var result = Market.Create(_credentials, null!, _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeEmpty()
    {
        var result = Market.Create(_credentials, string.Empty, _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeWhiteSpace()
    {
        var result = Market.Create(_credentials, " ", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbers()
    {
        var result = Market.Create(_credentials, "1.1", _onUpdate);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("1.1");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbersAndNotLetters()
    {
        var result = Market.Create(_credentials, "1.a", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must start with a '1.' followed by numbers.");
    }
}
