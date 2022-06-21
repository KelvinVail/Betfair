namespace Betfair.RestApi.Tests;

public class GetSessionTokenTests
{
    private readonly HttpClient _client = new ();

    [Fact]
    public void ThrowIfClientIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new SessionService(null, Credential.Create("x", "x", "x").Value));
        Assert.Equal("client", ex.ParamName);
    }

    [Fact]
    public void ThrowIfCredentialIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new SessionService(_client, null));
        Assert.Equal("credential", ex.ParamName);
    }
}