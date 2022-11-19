using System.Diagnostics.CodeAnalysis;
using Betfair.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace Betfair.Tests.Helpers;

public static class Errors
{
    public static void ShouldBe<T>(this Result<T, ErrorResult> result, [NotNull]ErrorResult error)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Error.Message.Should().Be(error.Message);
    }
}
