using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#pragma warning disable SA1010

namespace Betfair.Tests.Core.Client.TestDoubles;

public class HttpMessageHandlerSpy : HttpClientHandler
{
    public HttpMessageHandlerSpy() =>
        CheckCertificateRevocationList = true;

    public HttpStatusCode RespondsWitHttpStatusCode { get; set; } = HttpStatusCode.OK;

    public object? RespondsWithBody { get; set; }

    public HttpMethod? MethodUsed { get; private set; }

    public Uri? UriCalled { get; private set; }

    public Dictionary<Uri, int> TimesUriCalled { get; private set; } = [];

    public HttpRequestHeaders? HeadersSent { get; private set; }

    public HttpContentHeaders? ContentHeadersSent { get; private set; }

    public string? StringContentSent { get; private set; }

    public HttpContent? RawContentSent { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        [NotNull]HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        MethodUsed = request.Method;
        UriCalled = request.RequestUri;
        TimesUriCalled.TryAdd(request.RequestUri!, 0);
        TimesUriCalled[request.RequestUri!] += 1;

        HeadersSent = request.Headers;
        if (request.Content is not null)
        {
            ContentHeadersSent = request.Content.Headers;
            StringContentSent = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        var response = new HttpResponseMessage(RespondsWitHttpStatusCode);
        if (RespondsWithBody is null)
            return response;

        var bodyString = JsonSerializer.Serialize(RespondsWithBody, RespondsWithBody.GetInternalContext());
        response.Content = new StringContent(bodyString);

        request.Dispose();
        return response;
    }
}