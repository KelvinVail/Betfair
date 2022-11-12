using Betfair.Errors;

namespace Betfair.Tests.Errors;

public sealed class ErrorResultTests
{
    [Theory]
    [InlineData("THIS_IS_UNKNOWN", "This is unknown.")]
    [InlineData("UNEXPECTED_ERROR", "Unexpected error.")]
    public void ErrorCodeIsHumanizedIfItIsNotKnown(string code, string message)
    {
        var error = ErrorResult.Create(code);

        Assert.Equal(message, error.Message);
    }

    [Fact]
    public void InvalidUserNameOrPasswordReturnDescription()
    {
        var error = ErrorResult.Create("INVALID_USERNAME_OR_PASSWORD");

        Assert.Equal("The username or password is invalid.", error.Message);
    }

    [Fact]
    public void EmptyErrorReturnsDefaultValues()
    {
        var error = ErrorResult.Empty();

        Assert.Equal("value.must.not.be.empty", error.Code);
        Assert.Equal("'Value' must not be empty.", error.Message);
    }

    [Theory]
    [InlineData("parameterName", "Parameter Name")]
    [InlineData("input", "Input")]
    [InlineData("variable", "Variable")]
    public void EmptyErrorDisplaysParameterNameInTitleCase(string parameterName, string expected)
    {
        var error = ErrorResult.Empty(parameterName);

        Assert.Equal($"'{expected}' must not be empty.", error.Message);
    }

    [Fact]
    public void TwoErrorsWithTheSameErrorCodeAreEqual()
    {
        var error1 = ErrorResult.Empty();
        var error2 = ErrorResult.Empty("ParameterName");

        Assert.Equal(error1, error2);
        Assert.True(error1 == error2);
        Assert.True(error1.Equals(error2));
    }

    [Fact]
    public void TwoErrorsWithDifferentErrorCodesAreNotEqual()
    {
        var error1 = ErrorResult.Create("one");
        var error2 = ErrorResult.Create("two");

        Assert.NotEqual(error1, error2);
        Assert.True(error1 != error2);
        Assert.False(error1.Equals(error2));
    }
}