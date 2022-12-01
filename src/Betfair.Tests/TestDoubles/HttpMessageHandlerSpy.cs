using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Betfair.Tests.TestDoubles;

public class HttpMessageHandlerSpy : HttpMessageHandler
{
    private HttpRequestMessage _request;
    private string _requestContent = string.Empty;
    private string _responseBody = string.Empty;
    private HttpStatusCode _responseCode = HttpStatusCode.OK;
    private string _contentType = string.Empty;

    public void SetResponseBody(string body) =>
        _responseBody = body;

    public void SetResponseCode(HttpStatusCode code) =>
        _responseCode = code;

    public void AssertRequestMethod(HttpMethod method) =>
        Assert.Equal(method, _request.Method);

    public void AssertRequestUri(Uri uri) =>
        Assert.Equal(uri, _request.RequestUri);

    public void AssertRequestHeader(string key, string value) =>
        Assert.Contains(_request.Headers.GetValues(key), x => x == value);

    public void AssertContentHeader(string key, string value) =>
        Assert.Contains(_request.Content?.Headers.GetValues(key) !, x => x == value);

    public void AssertRequestContent(string body) =>
        Assert.Contains(body, _requestContent, StringComparison.CurrentCultureIgnoreCase);

    public void AssertContentType(string type) =>
        Assert.Equal(_contentType, type);

    protected override async Task<HttpResponseMessage> SendAsync(
        [NotNull]HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _request = request;
        if (request.Content is not null)
        {
            _requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
            _contentType = request.Content.Headers.ContentType?.ToString();
        }

        var response = new HttpResponseMessage(_responseCode);
        response.Content = new StringContent(_responseBody);
        return response;
    }
}