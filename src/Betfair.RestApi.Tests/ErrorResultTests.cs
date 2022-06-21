namespace Betfair.RestApi.Tests;

public sealed class ErrorResultTests
{
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

    [Theory]
    [InlineData("parameterValue")]
    [InlineData("input")]
    [InlineData("variable")]
    public void NotFoundErrorDisplaysParameterValue(string parameterValue)
    {
        var error = ErrorResult.NotFound(parameterValue);

        Assert.Equal("value.not.found", error.Code);
        Assert.Equal($"'{parameterValue}' not found.", error.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(329)]
    public void NotFoundErrorDisplaysParameterValueIfInt(int parameterValue)
    {
        var error = ErrorResult.NotFound(parameterValue);

        Assert.Equal($"'{parameterValue}' not found.", error.Message);
    }

    [Fact]
    public void NotFoundErrorDisplaysParameterValueIfValueObject()
    {
        var error = ErrorResult.NotFound("you@there.com");

        Assert.Equal("'you@there.com' not found.", error.Message);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 11)]
    [InlineData(2, 256)]
    public void BetweenErrorReturnsDefaultValues(int min, int max)
    {
        var error = ErrorResult.Between(min, max);

        Assert.Equal("value.to.short", error.Code);
        Assert.Equal($"'Value' must be between {min} and {max} characters long.", error.Message);
    }

    [Theory]
    [InlineData("parameterName", "Parameter Name")]
    [InlineData("input", "Input")]
    [InlineData("variable", "Variable")]
    public void BetweenErrorDisplaysParameterNameInTitleCase(string parameterName, string expected)
    {
        var error = ErrorResult.Between(1, 50, parameterName);

        Assert.Equal($"'{expected}' must be between 1 and 50 characters long.", error.Message);
    }

    [Fact]
    public void InvalidErrorReturnsDefaultValues()
    {
        var error = ErrorResult.Invalid();

        Assert.Equal("value.must.be.valid", error.Code);
        Assert.Equal("'Value' must be valid.", error.Message);
    }

    [Theory]
    [InlineData("parameterName", "Parameter Name")]
    [InlineData("input", "Input")]
    [InlineData("variable", "Variable")]
    public void InvalidErrorDisplaysParameterNameInTitleCase(string parameterName, string expected)
    {
        var error = ErrorResult.Invalid(parameterName);

        Assert.Equal($"'{expected}' must be valid.", error.Message);
    }

    [Theory]
    [InlineData("must be longer.")]
    [InlineData("trt again.")]
    public void InvalidErrorDisplayCustomMessage(string message)
    {
        var error = ErrorResult.Invalid("Name", message);

        Assert.Equal($"'Name' {message}", error.Message);
    }
}