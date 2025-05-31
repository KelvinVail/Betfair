using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;

namespace BenchmarkTest
{
    [MemoryDiagnoser]
    [SimpleJob]
    public class DeserializerBenchmark
    {
        private byte[][] _testData = null!;
        private string[] _testStrings = null!;
        private BetfairStreamDeserializer _betfairDeserializer = null!;

        [GlobalSetup]
        public void Setup()
        {
            // Use embedded test data - typical MarketStream.txt patterns
            var testLines = new[]
            {
                "{\"op\":\"mcm\",\"id\":\"EPSBET_1234567\",\"initialClk\":\"123\",\"clk\":\"124\",\"conflateMs\":0,\"heartbeatMs\":5000,\"pt\":1234567890,\"ct\":\"SUB_IMAGE\",\"mc\":[{\"id\":\"1.123456789\",\"marketDefinition\":{\"bspMarket\":false,\"turnInPlayEnabled\":true,\"persistenceEnabled\":true,\"marketBaseRate\":5.0,\"eventId\":\"12345678\",\"eventTypeId\":\"1\",\"numberOfWinners\":1,\"bettingType\":\"ODDS\",\"marketType\":\"MATCH_ODDS\",\"marketTime\":\"2023-01-01T15:00:00.000Z\",\"suspendTime\":\"2023-01-01T15:00:00.000Z\",\"bspReconciled\":false,\"complete\":true,\"inPlay\":false,\"crossMatching\":true,\"runnersVoidable\":false,\"numberOfActiveRunners\":2,\"betDelay\":0,\"status\":\"OPEN\",\"runners\":[{\"status\":\"ACTIVE\",\"adjustmentFactor\":1.0,\"id\":123456},{\"status\":\"ACTIVE\",\"adjustmentFactor\":1.0,\"id\":123457}],\"regulators\":[\"MR_INT\"],\"countryCode\":\"GB\",\"discountAllowed\":true,\"timezone\":\"GMT\",\"openDate\":\"2023-01-01T15:00:00.000Z\",\"version\":123456789},\"rc\":[{\"batb\":[[1.5,10.0],[1.6,20.0]],\"batl\":[[1.4,15.0],[1.3,25.0]],\"id\":123456},{\"batb\":[[2.5,30.0],[2.6,40.0]],\"batl\":[[2.4,35.0],[2.3,45.0]],\"id\":123457}],\"img\":true,\"tv\":100.0}]}",
                "{\"op\":\"mcm\",\"id\":\"EPSBET_1234567\",\"initialClk\":\"123\",\"clk\":\"125\",\"conflateMs\":0,\"heartbeatMs\":5000,\"pt\":1234567891,\"ct\":\"UPDATE\",\"mc\":[{\"id\":\"1.123456789\",\"rc\":[{\"batb\":[[1.51,12.0]],\"id\":123456}],\"tv\":102.0}]}",
                "{\"op\":\"mcm\",\"id\":\"EPSBET_1234567\",\"initialClk\":\"123\",\"clk\":\"126\",\"conflateMs\":0,\"heartbeatMs\":5000,\"pt\":1234567892,\"ct\":\"UPDATE\",\"mc\":[{\"id\":\"1.123456789\",\"rc\":[{\"batl\":[[1.39,18.0]],\"id\":123456},{\"batb\":[[2.51,32.0]],\"id\":123457}],\"tv\":105.0}]}",
                "{\"op\":\"mcm\",\"id\":\"EPSBET_1234567\",\"initialClk\":\"123\",\"clk\":\"127\",\"conflateMs\":0,\"heartbeatMs\":5000,\"pt\":1234567893,\"ct\":\"UPDATE\",\"mc\":[{\"id\":\"1.123456789\",\"rc\":[],\"tv\":105.0}]}",
                "{\"op\":\"mcm\",\"id\":\"EPSBET_1234567\",\"initialClk\":\"123\",\"clk\":\"128\",\"conflateMs\":0,\"heartbeatMs\":5000,\"pt\":1234567894,\"ct\":\"UPDATE\",\"mc\":[]}"
            };

            // Repeat the test data to get more samples
            var expandedLines = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                expandedLines.AddRange(testLines);
            }

            _testStrings = expandedLines.ToArray();
            _testData = _testStrings.Select(line => Encoding.UTF8.GetBytes(line)).ToArray();
            _betfairDeserializer = new BetfairStreamDeserializer();

            Console.WriteLine($"Loaded {_testData.Length} test messages for benchmarking");
        }

        [Benchmark(Baseline = true)]
        public ChangeMessage[] SystemTextJson()
        {
            var results = new ChangeMessage[_testStrings.Length];
            for (int i = 0; i < _testStrings.Length; i++)
            {
                results[i] = JsonSerializer.Deserialize<ChangeMessage>(_testStrings[i])!;
            }
            return results;
        }

        [Benchmark]
        public ChangeMessage[] BetfairOptimized()
        {
            var results = new ChangeMessage[_testData.Length];
            for (int i = 0; i < _testData.Length; i++)
            {
                results[i] = _betfairDeserializer.DeserializeChangeMessage(_testData[i])!;
            }
            return results;
        }

        [Benchmark]
        public void SystemTextJsonVoid()
        {
            for (int i = 0; i < _testStrings.Length; i++)
            {
                JsonSerializer.Deserialize<ChangeMessage>(_testStrings[i]);
            }
        }

        [Benchmark]
        public void BetfairOptimizedVoid()
        {
            for (int i = 0; i < _testData.Length; i++)
            {
                _betfairDeserializer.DeserializeChangeMessage(_testData[i]);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DeserializerBenchmark>();
        }
    }
}
