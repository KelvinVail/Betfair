using CSharpFunctionalExtensions;

namespace Betfair.RestApi.Tests;

public class CredentialTests
{
    [Fact]
    public void AppKeyMustNotBeNull() =>
        AssertAppKeyEmptyResult(Credential.Create(null, "username", "password"));

    [Fact]
    public void AppKeyMustNotBeEmpty() =>
        AssertAppKeyEmptyResult(Credential.Create(string.Empty, "username", "password"));

    [Fact]
    public void AppKeyMustNotBeWhitespace() =>
        AssertAppKeyEmptyResult(Credential.Create(" ", "username", "password"));

    [Fact]
    public void UsernameMustNotBeNull() =>
        AssertUsernameEmptyResult(Credential.Create("appKey", null, "password"));

    [Fact]
    public void UsernameMustNotBeEmpty() =>
        AssertUsernameEmptyResult(Credential.Create("appKey", string.Empty, "password"));

    [Fact]
    public void UsernameMustNotBeWhitespace() =>
        AssertUsernameEmptyResult(Credential.Create("appKey", " ", "password"));

    [Fact]
    public void PasswordMustNotBeNull() =>
        AssertPasswordEmptyResult(Credential.Create("appKey", "username", null));

    [Fact]
    public void PasswordMustNotBeEmpty() =>
        AssertPasswordEmptyResult(Credential.Create("appKey", "username", string.Empty));

    [Fact]
    public void PasswordMustNotBeWhitespace() =>
        AssertPasswordEmptyResult(Credential.Create("appKey", "username", " "));

    [Fact]
    public void TwoCredentialsWithTheSameValuesAreEqual()
    {
        var cred1 = Credential.Create("appKey", "username", "password").Value;
        var cred2 = Credential.Create("appKey", "username", "password").Value;

        Assert.Equal(cred1, cred2);
        Assert.True(cred1 == cred2);
        Assert.True(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentAppKeysAreNotEqual()
    {
        var cred1 = Credential.Create("appKey", "username", "password").Value;
        var cred2 = Credential.Create("other", "username", "password").Value;

        Assert.NotEqual(cred1, cred2);
        Assert.True(cred1 != cred2);
        Assert.False(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentUsernamesAreNotEqual()
    {
        var cred1 = Credential.Create("appKey", "username", "password").Value;
        var cred2 = Credential.Create("appKey", "other", "password").Value;

        Assert.NotEqual(cred1, cred2);
        Assert.True(cred1 != cred2);
        Assert.False(cred1.Equals(cred2));
    }

    [Fact]
    public void TwoCredentialsWithDifferentPasswordsAreNotEqual()
    {
        var cred1 = Credential.Create("appKey", "username", "password").Value;
        var cred2 = Credential.Create("appKey", "username", "other").Value;

        Assert.NotEqual(cred1, cred2);
        Assert.True(cred1 != cred2);
        Assert.False(cred1.Equals(cred2));
    }

    private static void AssertAppKeyEmptyResult(Result<Credential, ErrorResult> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorResult.Empty("appKey"), result);
        Assert.Equal("'App Key' must not be empty.", result.Error.Message);
    }

    private static void AssertUsernameEmptyResult(Result<Credential, ErrorResult> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorResult.Empty("username"), result);
        Assert.Equal("'Username' must not be empty.", result.Error.Message);
    }

    private static void AssertPasswordEmptyResult(Result<Credential, ErrorResult> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorResult.Empty("password"), result);
        Assert.Equal("'Password' must not be empty.", result.Error.Message);
    }
}