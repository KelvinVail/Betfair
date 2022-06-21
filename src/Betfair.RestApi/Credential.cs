using CSharpFunctionalExtensions;

namespace Betfair.RestApi;

public sealed class Credential : ValueObject
{
    private readonly string _appKey;
    private readonly string _username;
    private readonly string _password;

    private Credential(string appKey, string username, string password)
    {
        _appKey = appKey;
        _username = username;
        _password = password;
    }

    public static Result<Credential, ErrorResult> Create(string appKey, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(appKey)) return ErrorResult.Empty(nameof(appKey));
        if (string.IsNullOrWhiteSpace(username)) return ErrorResult.Empty(nameof(username));
        if (string.IsNullOrWhiteSpace(password)) return ErrorResult.Empty(nameof(password));

        return new Credential(appKey, username, password);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _appKey;
        yield return _username;
        yield return _password;
    }
}