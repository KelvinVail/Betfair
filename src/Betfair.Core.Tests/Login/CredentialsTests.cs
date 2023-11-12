using Betfair.Core.Login;

namespace Betfair.Core.Tests.Login;

public sealed class CredentialsTests
{
    [Fact]
    public void UsernameMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials(null!, "password", "appKey"));

        ex.ParamName.Should().Be("username");
    }

    [Fact]
    public void UsernameMustNotBeEmpty()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials(string.Empty, "password", "appKey"));

        ex.ParamName.Should().Be("username");
    }

    [Fact]
    public void UsernameMustNotBeWhitespace()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials(" ", "password", "appKey"));

        ex.ParamName.Should().Be("username");
    }

    [Fact]
    public void PasswordMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", null!, "appKey"));

        ex.ParamName.Should().Be("password");
    }

    [Fact]
    public void PasswordMustNotBeEmpty()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", string.Empty, "appKey"));

        ex.ParamName.Should().Be("password");
    }

    [Fact]
    public void PasswordMustNotBeWhitespace()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", " ", "appKey"));

        ex.ParamName.Should().Be("password");
    }

    [Fact]
    public void AppKeyMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", "password", null!));

        ex.ParamName.Should().Be("appKey");
    }

    [Fact]
    public void AppKeyMustNotBeEmpty()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", "password", string.Empty));

        ex.ParamName.Should().Be("appKey");
    }

    [Fact]
    public void AppKeyMustNotBeWhitespace()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Credentials("username", "password", " "));

        ex.ParamName.Should().Be("appKey");
    }

    [Fact]
    public void GetLoginRequestShould()
    {
        var credentials = new Credentials("username", "password", "appKey");

        credentials.GetLoginRequest();
    }
}
