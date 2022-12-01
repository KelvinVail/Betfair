using Betfair.Errors;
using Betfair.Login;
using Betfair.Tests.TestDoubles;
using CSharpFunctionalExtensions;

namespace Betfair.Tests.Login;

public sealed class CredentialsTests
{
    [Fact]
    public void UsernameMustNotBeNull()
    {
        var result = Credentials.Create(null, "password", "appKey");

        AssertError(ErrorResult.Empty("username"), result);
    }

    [Fact]
    public void UsernameMustNotBeEmpty()
    {
        var result = Credentials.Create(string.Empty, "password", "appKey");

        AssertError(ErrorResult.Empty("username"), result);
    }

    [Fact]
    public void UsernameMustNotBeWhitespace()
    {
        var result = Credentials.Create(" ", "password", "appKey");

        AssertError(ErrorResult.Empty("username"), result);
    }

    [Fact]
    public void PasswordMustNotBeNull()
    {
        var result = Credentials.Create("username", null, "appKey");

        AssertError(ErrorResult.Empty("password"), result);
    }

    [Fact]
    public void PasswordMustNotBeEmpty()
    {
        var result = Credentials.Create("username", string.Empty, "appKey");

        AssertError(ErrorResult.Empty("password"), result);
    }

    [Fact]
    public void PasswordMustNotBeWhitespace()
    {
        var result = Credentials.Create("username", " ", "appKey");

        AssertError(ErrorResult.Empty("password"), result);
    }

    [Fact]
    public void AppKeyMustNotBeNull()
    {
        var result = Credentials.Create("username", "password", null);

        AssertError(ErrorResult.Empty("appKey"), result);
    }

    [Fact]
    public void AppKeyMustNotBeEmpty()
    {
        var result = Credentials.Create("username", "password", string.Empty);

        AssertError(ErrorResult.Empty("appKey"), result);
    }

    [Fact]
    public void AppKeyMustNotBeWhitespace()
    {
        var result = Credentials.Create("username", "password", " ");

        AssertError(ErrorResult.Empty("appKey"), result);
    }

    [Fact]
    public void TwoCredentialsWithTheSameUsernamePasswordAndAppKeyAreEqual()
    {
        var cred1 = Credentials.Create("username", "password", "appKey").Value;
        var cred2 = Credentials.Create("username", "password", "appKey").Value;

        Assert.True(cred1 == cred2);
        Assert.Equal(cred1, cred2);
        Assert.True(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentUsernamesAreNotEqual()
    {
        var cred1 = Credentials.Create("username", "password", "appKey").Value;
        var cred2 = Credentials.Create("username2", "password", "appKey").Value;

        Assert.True(cred1 != cred2);
        Assert.NotEqual(cred1, cred2);
        Assert.False(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentPasswordsAreNotEqual()
    {
        var cred1 = Credentials.Create("username", "password", "appKey").Value;
        var cred2 = Credentials.Create("username", "password2", "appKey").Value;

        Assert.True(cred1 != cred2);
        Assert.NotEqual(cred1, cred2);
        Assert.False(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentAppKeysAreNotEqual()
    {
        var cred1 = Credentials.Create("username", "password", "appKey").Value;
        var cred2 = Credentials.Create("username", "password", "appKey2").Value;

        Assert.True(cred1 != cred2);
        Assert.NotEqual(cred1, cred2);
        Assert.False(cred1.Equals(cred2));
    }

    [Fact]
    public void CreateWithCertReturnsErrors()
    {
        using var cert = new X509Certificate2Stub();
        var result = Credentials.Create(
            string.Empty,
            "password",
            "appKey",
            cert);

        AssertError(ErrorResult.Empty("username"), result);
    }

    [Fact]
    public void CertificationMustNotBeNull()
    {
        var result = Credentials.Create(
            "username",
            "password",
            "appKey",
            null);

        AssertError(ErrorResult.Empty("certificate"), result);
    }

    private static void AssertError(ErrorResult expected, Result<object, ErrorResult> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expected, result.Error);
        Assert.Equal(expected.Message, result.Error.Message);
    }
}
