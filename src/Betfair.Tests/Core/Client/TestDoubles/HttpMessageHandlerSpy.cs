using System.Diagnostics.CodeAnalysis;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Core.Client.TestDoubles;

public class HttpMessageHandlerSpy : HttpClientHandler
{
    public HttpStatusCode RespondsWitHttpStatusCode { get; set; } = HttpStatusCode.OK;

    public object? RespondsWithBody { get; set; }

    public HttpMethod? MethodUsed { get; private set; }

    public Uri? UriCalled { get; private set; }

    public Dictionary<Uri, int> TimesUriCalled { get; private set; } = new ();

    public HttpRequestHeaders? HeadersSent { get; private set; }

    public HttpContentHeaders? ContentHeadersSent { get; private set; }

    public object? ContentSent { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        [NotNull]HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        MethodUsed = request.Method;
        UriCalled = request.RequestUri;
        if (!TimesUriCalled.ContainsKey(request.RequestUri!))
            TimesUriCalled.Add(request.RequestUri!, 0);

        TimesUriCalled[request.RequestUri!] += 1;

        HeadersSent = request.Headers;
        if (request.Content is not null)
        {
            ContentHeadersSent = request.Content.Headers;
            if (IsFormUrlEncoded(request))
                ContentSent = await request.Content.ReadAsStringAsync(cancellationToken);
            else
                ContentSent = JsonSerializer.Deserialize<object>(await request.Content.ReadAsStreamAsync(cancellationToken), StandardResolver.AllowPrivateExcludeNullCamelCase);
        }

        var response = new HttpResponseMessage(RespondsWitHttpStatusCode);
        if (RespondsWithBody is null)
            return response;

        var bodyString = JsonSerializer.ToJsonString(RespondsWithBody, StandardResolver.CamelCase);
        response.Content = new StringContent(bodyString);

        request.Dispose();
        return response;
    }

    private static bool IsFormUrlEncoded(HttpRequestMessage request) =>
        Equals(request.Content?.Headers.ContentType, new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
}