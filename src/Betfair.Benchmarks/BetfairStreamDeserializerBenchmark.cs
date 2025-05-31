using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Betfair.Stream.Deserializers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Betfair.Benchmarks;

public class BetfairStreamDeserializerBenchmark
{
    private byte[] _testData = null!;
    private BetfairStreamDeserializer _optimizedDeserializer = null!;

    public void Setup()
    {
        // Sample Betfair stream data
        var sampleJson = """
        {
            "op": "mcm",
            "id": 1,
            "initialClk": "123",
            "clk": "AAABCD",
            "conflateMs": 0,
            "heartbeatMs": 5000,
            "pt": 1234567890,
            "ct": "SUB_IMAGE",
            "mc": [
                {
                    "id": "1.123456789",
                    "marketDefinition": {
                        "bspMarket": false,
                        "turnInPlayEnabled": true,
                        "persistenceEnabled": true,
                        "marketBaseRate": 5.0,
                        "eventId": "12345",
                        "eventTypeId": "1",
                        "numberOfWinners": 1,
                        "bettingType": "ODDS",
                        "marketType": "MATCH_ODDS",
                        "marketTime": "2023-01-01T12:00:00.000Z",
                        "suspendTime": "2023-01-01T12:00:00.000Z",
                        "bspReconciled": false,
                        "complete": true,
                        "inPlay": false,
                        "crossMatching": true,
                        "runnersVoidable": false,
                        "numberOfActiveRunners": 2,
                        "betDelay": 0,
                        "status": "OPEN",
                        "runners": [
                            {
                                "status": "ACTIVE",
                                "adjustmentFactor": 1.0,
                                "id": 47972,
                                "sortPriority": 1
                            },
                            {
                                "status": "ACTIVE", 
                                "adjustmentFactor": 1.0,
                                "id": 47973,
                                "sortPriority": 2
                            }
                        ],
                        "regulators": ["MR_INT"],
                        "countryCode": "GB",
                        "discountAllowed": true,
                        "timezone": "GMT",
                        "openDate": "2023-01-01T12:00:00.000Z",
                        "version": 1
                    },
                    "rc": [
                        {
                            "batb": [[1.01, 1000], [1.02, 2000]],
                            "batl": [[1.03, 1500], [1.04, 2500]],
                            "id": 47972
                        },
                        {
                            "batb": [[2.0, 500], [2.1, 1000]],
                            "batl": [[2.2, 750], [2.3, 1250]],
                            "id": 47973
                        }
                    ],
                    "img": true
                }
            ]
        }
        """;

        _testData = Encoding.UTF8.GetBytes(sampleJson);
        _optimizedDeserializer = new BetfairStreamDeserializer();
    }

    [Benchmark(Baseline = true)]
    public object? SystemTextJson()
    {
        return JsonSerializer.Deserialize<Betfair.Stream.Responses.ChangeMessage>(_testData);
    }

    [Benchmark]
    public object? OptimizedDeserializer()
    {
        return _optimizedDeserializer.DeserializeChangeMessage(_testData);
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<BetfairStreamDeserializerBenchmark>();
    }
}
