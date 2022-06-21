namespace Betfair.RestApi.Tests;

public class GetSessionTokenTests
{
    [Fact]
    public void ThrowIfClientIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new SessionService(null));
        Assert.Equal("client", ex.ParamName);
    }
}