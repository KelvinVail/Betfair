using Humanizer;

namespace Betfair.Errors;

public sealed class ErrorResult : ValueObject
{
    private static readonly Dictionary<string, string> _descriptions = new ()
    {
        { "INVALID_USERNAME_OR_PASSWORD", "The username or password is invalid." },
    };

    private ErrorResult(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public static ErrorResult Create(string code) =>
        _descriptions.ContainsKey(code)
            ? new ErrorResult(code, _descriptions[code])
            : new ErrorResult(
                code,
                code.Humanize().Transform(To.LowerCase, To.SentenceCase) + ".");

    public static ErrorResult Empty(string? paramName = null) =>
        new (
            "value.must.not.be.empty",
            $"'{Humanize(paramName)}' must not be empty.");

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Code;
    }

    private static string Humanize(string? paramName = null) =>
        paramName?.Humanize().Transform(To.TitleCase) ?? "Value";
}