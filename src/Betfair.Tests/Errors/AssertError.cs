using System.Diagnostics.CodeAnalysis;
using Betfair.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace Betfair.Tests.Errors;

public static class AssertError
{
    public static void ShouldBeFailure(this UnitResult<ErrorResult> result, [NotNull]ErrorResult error)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Error.Message.Should().Be(error.Message);
    }

    public static void ShouldBeFailure(this UnitResult<ErrorResult> result) =>
        result.IsFailure.Should().BeTrue();

    public static void ShouldBeFailure<T>(this Result<T, ErrorResult> result, [NotNull] ErrorResult error)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Error.Message.Should().Be(error.Message);
    }

    public static void ShouldBeFailure<T>(this Result<T, ErrorResult> result) =>
        result.IsFailure.Should().BeTrue();

    public static void ShouldBeSuccess(this UnitResult<ErrorResult> result) =>
        result.IsSuccess.Should().BeTrue();

    public static void ShouldBeSuccess<T>(this Result<T, ErrorResult> result) =>
        result.IsSuccess.Should().BeTrue();
}