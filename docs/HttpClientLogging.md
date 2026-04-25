# HTTP Client Logging

By default `BetfairApiClient` manages its own `HttpClient` internally. If you want to plug into the .NET HTTP client pipeline — for example to add structured request/response logging, metrics, or other delegating handlers — you can supply an `IHttpClientFactory` instead.

## Contents
- [Why use IHttpClientFactory](#why-use-ihttpclientfactory)
- [Basic setup](#basic-setup)
- [Adding extended HTTP logging](#adding-extended-http-logging)
- [Configuring what gets logged](#configuring-what-gets-logged)
- [Adding phase-level latency telemetry](#adding-phase-level-latency-telemetry)
  - [What gets captured](#what-gets-captured)
  - [Full setup with logging and latency](#full-setup-with-logging-and-latency)
  - [Reading LatencyInfo in logs](#reading-latencyinfo-in-logs)
  - [Exporting phase timings as metrics](#exporting-phase-timings-as-metrics)
- [Consuming the logs](#consuming-the-logs)
  - [Console (development)](#console-development)
  - [Application Insights](#application-insights)
  - [Seq](#seq)
- [Adding custom log enrichers](#adding-custom-log-enrichers)

## Why use IHttpClientFactory

The default `BetfairApiClient(Credentials)` constructor is the simplest way to get started and requires no DI setup. The factory-based constructor exists for when you need visibility into outgoing HTTP traffic — latency, status codes, request bodies — without writing your own middleware.

## Basic setup

Register a named `"Betfair"` client with your DI container and pass the factory to `BetfairApiClient`.

```csharp
using Betfair.Api;
using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Microsoft.Extensions.DependencyInjection;

var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY");

builder.Services.AddHttpClient("Betfair")
    .ConfigurePrimaryHttpMessageHandler(() => new BetfairClientHandler());

builder.Services.AddSingleton(sp =>
    new BetfairApiClient(credentials, sp.GetRequiredService<IHttpClientFactory>()));
```

`BetfairClientHandler` is the same handler used internally by the default constructor. It enables certificate revocation checking, GZip decompression, and disables proxy usage. If you are using certificate-based authentication, pass your certificate to it:

```csharp
var cert = X509Certificate2.CreateFromPemFile("betfair.crt", "betfair.key");

builder.Services.AddHttpClient("Betfair")
    .ConfigurePrimaryHttpMessageHandler(() => new BetfairClientHandler(cert));
```

## Adding extended HTTP logging

Install the diagnostics package:

```bash
dotnet add package Microsoft.Extensions.Http.Diagnostics
dotnet add package Microsoft.Extensions.Compliance.Redaction
```

Then register the extended logger alongside your client:

```csharp
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection;

// Required by AddExtendedHttpClientLogging
builder.Services.AddRedaction();

builder.Services.AddHttpClient("Betfair")
    .ConfigurePrimaryHttpMessageHandler(() => new BetfairClientHandler())
    .AddExtendedHttpClientLogging(options =>
    {
        // Log request and response bodies (use with care in production — contains bet data)
        options.LogBody = true;
        options.RequestBodyContentTypes.Add("application/json");
        options.ResponseBodyContentTypes.Add("application/json");

        // Cap body size to avoid large object heap pressure
        options.BodySizeLimit = 4096;

        // Log the full request path (disable redaction for internal tooling)
        options.RequestPathParameterRedactionMode =
            HttpRouteParameterRedactionMode.None;
    });

builder.Services.AddSingleton(sp =>
    new BetfairApiClient(credentials, sp.GetRequiredService<IHttpClientFactory>()));
```

This adds a structured log entry for every request made through `BetfairApiClient`, including:

| Field | Example |
|---|---|
| `http.request.method` | `POST` |
| `http.request.body` | `{"filter":{"marketIds":["1.234"]}}` |
| `url.full` | `https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/` |
| `http.response.status_code` | `200` |
| `http.response.body` | `[{"marketId":"1.234",...}]` |
| `duration` | `42` (ms) |

## Configuring what gets logged

You can tune logging to match your environment. A minimal production configuration that captures latency and status codes without logging bodies:

```csharp
.AddExtendedHttpClientLogging(options =>
{
    // No body logging — avoids leaking bet data into log storage
    options.LogBody = false;

    // Log the Content-Type header to confirm JSON is being sent/received
    options.RequestHeadersDataClasses.Add("Content-Type", DataClassification.None);
    options.ResponseHeadersDataClasses.Add("Content-Type", DataClassification.None);
});
```

A more detailed configuration for debugging:

```csharp
.AddExtendedHttpClientLogging(options =>
{
    options.LogBody = true;
    options.RequestBodyContentTypes.Add("application/json");
    options.ResponseBodyContentTypes.Add("application/json");
    options.BodySizeLimit = 16384;
    options.BodyReadTimeout = TimeSpan.FromSeconds(2);

    // Log the session token header name (not value — redacted by default)
    options.RequestHeadersDataClasses.Add("X-Authentication", DataClassification.Unknown);
    options.RequestHeadersDataClasses.Add("X-Application", DataClassification.None);
    options.RequestHeadersDataClasses.Add("Content-Type", DataClassification.None);
    options.ResponseHeadersDataClasses.Add("Content-Type", DataClassification.None);

    // Log a start entry before the request is sent, plus the final entry on completion
    options.LogRequestStart = true;
});
```

## Adding phase-level latency telemetry

`AddExtendedHttpClientLogging` records total request duration. `AddHttpClientLatencyTelemetry` goes further — it captures a timestamp at each stage of the HTTP lifecycle so you can see exactly where time is being spent: DNS lookup, TCP connect, TLS handshake, writing headers, streaming the body, waiting for the first response byte, and reading the response.

No library changes are needed. This is entirely consumer-side configuration that plugs into the `IHttpClientFactory` pipeline you already have.

Install the package (same one used for extended logging):

```bash
dotnet add package Microsoft.Extensions.Http.Diagnostics
```

### What gets captured

| Phase | What it measures |
|---|---|
| `Http.NameResolutionStart/End` | DNS lookup for `api.betfair.com` (usually cached after first request) |
| `Http.SocketConnectStart/End` | TCP connection establishment |
| `Http.ConnectionEstablished` | Usable connection ready (after TLS handshake) |
| `Http.RequestHeadersStart/End` | Time to write request headers to the wire |
| `Http.RequestContentStart/End` | Time to stream the request body (your filter JSON) |
| `Http.ResponseHeadersStart/End` | Time from first byte to full header parse |
| `Http.ResponseContentStart/End` | Time to read the full response body |
| `Http.GCPauseTime` | GC pause duration overlapping the request (.NET 8+ only) |
| `Http.ConnectionInitiated` | Emitted when a new connection is opened (not a pool reuse) |

### Full setup with logging and latency

Registration order matters: `AddHttpClientLatencyTelemetry` must come before `AddExtendedHttpClientLogging` so the latency handler wraps the logging handler in the pipeline.

```csharp
using Betfair.Api;
using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Diagnostics;

var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY");

// Required by AddExtendedHttpClientLogging
builder.Services.AddRedaction();

// Required by AddHttpClientLatencyTelemetry
builder.Services.AddLatencyContext();

// 1. Latency telemetry first — its handler must be outermost in the pipeline
builder.Services.AddHttpClientLatencyTelemetry(options =>
{
    // Capture all phase timings (default). Set to false only in extremely
    // high-throughput scenarios where even measurement overhead matters.
    options.EnableDetailedLatencyBreakdown = true;
});

// 2. Extended logging second — reads the completed latency context and
//    serialises it into the LatencyInfo structured log field
builder.Services.AddExtendedHttpClientLogging(options =>
{
    options.LogBody = false; // keep off in production
    options.RequestHeadersDataClasses.Add("Content-Type", DataClassification.None);
    options.ResponseHeadersDataClasses.Add("Content-Type", DataClassification.None);
});

// 3. Register the named Betfair client
builder.Services.AddHttpClient("Betfair")
    .ConfigurePrimaryHttpMessageHandler(() => new BetfairClientHandler());

builder.Services.AddSingleton(sp =>
    new BetfairApiClient(credentials, sp.GetRequiredService<IHttpClientFactory>()));
```

### Reading LatencyInfo in logs

When both extensions are registered together, each log entry gains a `LatencyInfo` field containing the full phase breakdown as a compact string:

```json
{
  "url.full": "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/",
  "http.response.status_code": 200,
  "duration": 42,
  "LatencyInfo": "v1;s=api.betfair.com;t=Http.NameResolutionStart:0,Http.NameResolutionEnd:1,Http.SocketConnectStart:1,Http.SocketConnectEnd:18,Http.ConnectionEstablished:19,Http.RequestHeadersStart:19,Http.RequestHeadersEnd:20,Http.RequestContentStart:20,Http.RequestContentEnd:21,Http.ResponseHeadersStart:21,Http.ResponseHeadersEnd:38,Http.ResponseContentStart:38,Http.ResponseContentEnd:42;m=Http.GCPauseTime:0"
}
```

The format is `v1;s=<server>;t=<checkpoint>:<ms>,...;m=<measure>:<value>,...`. All timestamps are milliseconds from the start of the request. In the example above you can see the TLS handshake took 17ms (`SocketConnectStart:1` to `ConnectionEstablished:19`) and the server took 17ms to return the first response byte (`RequestContentEnd:21` to `ResponseHeadersStart:21`).

In Seq you can filter on slow connection establishment:

```
LatencyInfo like '%Http.ConnectionInitiated%'
```

That line only appears when a new connection was opened rather than reused from the pool — useful for spotting connection churn.

### Exporting phase timings as metrics

The latency context accumulates data but emits nothing automatically beyond the `LatencyInfo` log field. To push phase timings into a metrics backend (Prometheus, OpenTelemetry, etc.) add a custom `DelegatingHandler` that reads `ILatencyContextAccessor` after the request completes:

```csharp
using Microsoft.Extensions.Http.Diagnostics;

public class BetfairLatencyExporter : DelegatingHandler
{
    private readonly ILatencyContextAccessor _latency;
    private readonly IMeterFactory _meterFactory; // or your preferred metrics abstraction

    public BetfairLatencyExporter(
        ILatencyContextAccessor latency,
        IMeterFactory meterFactory)
    {
        _latency = latency;
        _meterFactory = meterFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct)
    {
        var response = await base.SendAsync(request, ct);

        var ctx = _latency.Current;
        if (ctx is not null)
        {
            var data = ctx.LatencyData;

            // Record total duration as a histogram dimension by endpoint
            var endpoint = request.RequestUri?.AbsolutePath ?? "unknown";
            RecordHistogram("betfair.http.duration_ms", data.DurationMs, endpoint);

            // Record GC pause overlap (available on .NET 8+)
            if (TryGetMeasure(data, "Http.GCPauseTime", out var gcPause))
                RecordHistogram("betfair.http.gc_pause_ms", gcPause, endpoint);

            // Count new connection openings (pool misses)
            if (HasCheckpoint(data, "Http.ConnectionInitiated"))
                IncrementCounter("betfair.http.new_connections", endpoint);
        }

        return response;
    }

    // Implement RecordHistogram / IncrementCounter using your metrics library
}
```

Register it after the latency telemetry handler so it runs inside the pipeline and sees the finalised context:

```csharp
builder.Services.AddTransient<BetfairLatencyExporter>();

builder.Services.AddHttpClient("Betfair")
    .ConfigurePrimaryHttpMessageHandler(() => new BetfairClientHandler())
    .AddHttpMessageHandler<BetfairLatencyExporter>();
```

## Consuming the logs

The extended logger writes structured log entries using the standard `ILogger` infrastructure. The data is added as tags on the log entry rather than embedded in the message string, so you need a structured logging provider to query it.

### Console (development)

Add JSON console output to see the enriched tags during development:

```csharp
builder.Logging.AddJsonConsole(options =>
{
    options.JsonWriterOptions = new JsonWriterOptions { Indented = true };
});
```

A completed request will produce output like:

```json
{
  "Timestamp": "2025-04-25T10:32:01.123Z",
  "EventId": { "Id": 100, "Name": "RequestStop" },
  "LogLevel": "Information",
  "Category": "System.Net.Http.HttpClient.Betfair.ClientHandler",
  "Message": "Sending HTTP request POST https://api.betfair.com/...",
  "State": {
    "http.request.method": "POST",
    "url.full": "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/",
    "http.response.status_code": 200,
    "duration": 38,
    "http.request.body": "{\"marketIds\":[\"1.234\"]}",
    "http.response.body": "[{\"marketId\":\"1.234\",\"status\":\"OPEN\"}]"
  }
}
```

### Application Insights

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

HTTP dependency calls from `BetfairApiClient` will appear in the **Dependencies** blade in the Azure portal. The enriched tags are available as custom properties on each dependency telemetry item, letting you filter by endpoint, status code, or duration.

### Seq

```csharp
builder.Logging.AddSeq("http://localhost:5341");
```

In Seq you can query by structured property, for example to find all slow Betfair calls:

```
duration > 500 and url.full like '%betfair.com%'
```

Or to find all failed requests:

```
http.response.status_code >= 400 and url.full like '%betfair.com%'
```

## Adding custom log enrichers

If you want to attach additional context to every Betfair HTTP log entry — for example a strategy name or market ID extracted from the request body — implement `IHttpClientLogEnricher`:

```csharp
using Microsoft.Extensions.Http.Diagnostics;

public class BetfairLogEnricher : IHttpClientLogEnricher
{
    public void Enrich(
        IEnrichmentTagCollector collector,
        HttpRequestMessage request,
        HttpResponseMessage? response,
        Exception? exception)
    {
        // Tag slow responses for easy filtering
        if (response is not null)
        {
            var statusCode = (int)response.StatusCode;
            if (statusCode >= 400)
                collector.Add("betfair.error", true);
        }

        if (exception is not null)
            collector.Add("betfair.exception_type", exception.GetType().Name);
    }
}

// Register alongside the client
builder.Services.AddHttpClientLogEnricher<BetfairLogEnricher>();
```
