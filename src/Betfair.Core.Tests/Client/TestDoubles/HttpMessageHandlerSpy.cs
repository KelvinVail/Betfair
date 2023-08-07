﻿using System.Diagnostics.CodeAnalysis;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Core.Tests.Client.TestDoubles;

public class HttpMessageHandlerSpy : HttpMessageHandler
{
    public HttpStatusCode RespondsWitHttpStatusCode { get; set; } = HttpStatusCode.OK;

    public object? RespondsWithBody { get; set; }

    public HttpMethod? MethodUsed { get; private set; }

    public Uri? UriCalled { get; private set; }

    public HttpRequestHeaders? HeadersSent { get; private set; }

    public HttpContentHeaders? ContentHeadersSent { get; private set; }

    public object? ContentSent { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        [NotNull]HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        MethodUsed = request.Method;
        UriCalled = request.RequestUri;
        HeadersSent = request.Headers;
        if (request.Content is not null)
        {
            ContentSent = await JsonSerializer.DeserializeAsync<object>(await request.Content.ReadAsStreamAsync(cancellationToken), StandardResolver.ExcludeNullCamelCase);
            ContentHeadersSent = request.Content.Headers;
        }

        var response = new HttpResponseMessage(RespondsWitHttpStatusCode);
        if (RespondsWithBody is null)
            return response;

        var bodyString = JsonSerializer.ToJsonString(RespondsWithBody, StandardResolver.CamelCase);
        response.Content = new StringContent(bodyString);

        return response;
    }
}