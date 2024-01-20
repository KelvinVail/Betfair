namespace Betfair.Core.Login;

/// <summary>
/// Used to store all information need to authenticate to Betfair.
/// </summary>
public sealed class Credentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Credentials"/> class.
    /// Used to store all information need to authenticate to Betfair.
    /// </summary>
    /// <param name="username">Your Betfair username.</param>
    /// <param name="password">Your Betfair password.</param>
    /// <param name="appKey">Your Betfair app key.</param>
    /// <param name="cert">An optional certificate. If provided the library will use the cert to authenticate to Betfair.</param>
    /// <exception cref="ArgumentNullException">Will throw if username, password or appKey is null, empty or whitespace.</exception>
    public Credentials(
        string username,
        string password,
        string appKey,
        X509Certificate2? cert = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(appKey))
            throw new ArgumentNullException(nameof(appKey));

        Username = username;
        Password = password;
        AppKey = appKey;
        Certificate = cert;
    }

    internal string Username { get; }

    internal string Password { get; }

    internal string AppKey { get; }

    internal X509Certificate2? Certificate { get; }
}
