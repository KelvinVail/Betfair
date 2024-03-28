﻿namespace Betfair.Benchmarks.JsonBenchmarks.Responses;

public struct ChangeMessage
{
    [JsonPropertyName("op")]
    public string? Operation { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; init; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; init; }

    [JsonPropertyName("connectionId")]
    public string? ConnectionId { get; init; }

    [JsonPropertyName("connectionClosed")]
    public bool? ConnectionClosed { get; init; }

    [JsonPropertyName("connectionsAvailable")]
    public int? ConnectionsAvailable { get; init; }

    [JsonPropertyName("initialClk")]
    public string? InitialClock { get; init; }

    [JsonPropertyName("clk")]
    public string? Clock { get; init; }

    [JsonPropertyName("conflateMs")]
    public int? ConflateMs { get; init; }

    [JsonPropertyName("heartbeatMs")]
    public int? HeartbeatMs { get; init; }

    [JsonPropertyName("pt")]
    public long? PublishTime { get; init; }

    [JsonPropertyName("ct")]
    public string? ChangeType { get; init; }

    [JsonPropertyName("segmentType")]
    public string? SegmentType { get; init; }

    [JsonPropertyName("mc")]
    public List<MarketChange>? MarketChanges { get; init; }

    [JsonPropertyName("oc")]
    public List<OrderChange>? OrderChanges { get; init; }

    [JsonPropertyName("rt")]
    public long ReceivedTick { get; internal set; }

    [JsonPropertyName("dt")]
    public long DeserializedTick { get; internal set; }
}