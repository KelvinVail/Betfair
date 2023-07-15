namespace Betfair.Core.Client;

[Serializable]
public class BetfairRequestException : Exception
{
    public BetfairRequestException()
    {
    }

    public BetfairRequestException(string message)
        : base(message)
    {
    }

    public BetfairRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public BetfairRequestException(
        HttpStatusCode? statusCode,
        string? message = null,
        Exception? inner = null)
        : base(message, inner) =>
        StatusCode = statusCode;

    protected BetfairRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public HttpStatusCode? StatusCode { get; set; }
}
