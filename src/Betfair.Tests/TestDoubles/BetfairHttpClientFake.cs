﻿using System.Net;
using Betfair.Client;
using Betfair.Errors;
using CSharpFunctionalExtensions;

namespace Betfair.Tests.TestDoubles;

public class BetfairHttpClientFake : BetfairHttpClient
{
    public Uri LastUriCalled { get; private set; }

    public string LastSessionTokenUsed { get; private set; }

    public object LastBodySent { get; private set; }

    public object SetResponse { get; set; } = default;

    public ErrorResult SetError { get; set; }

    public HttpResponseMessage SetResponseMessage { get; set; }

    public HttpRequestMessage LastMessageSent { get; private set; }

    public override async Task<Result<T, ErrorResult>> Post<T>(
        Uri uri,
        string sessionToken,
        object body = null,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        LastUriCalled = uri;
        LastSessionTokenUsed = sessionToken;
        LastBodySent = body;

        if (SetError != null) return SetError;
        return Result.Success<T, ErrorResult>((T)SetResponse);
    }

    public override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (request?.Content is not null)
            LastBodySent = await request.Content.ReadAsStringAsync(cancellationToken);

        LastMessageSent = request;

        return SetResponseMessage ?? new HttpResponseMessage(HttpStatusCode.OK);
    }
}
