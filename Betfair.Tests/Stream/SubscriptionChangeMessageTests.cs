namespace Betfair.Tests.Stream
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Betfair.Stream.Responses;
    using Xunit;

    public sealed class SubscriptionChangeMessageTests : SubscriptionTests
    {
        [Theory]
        [InlineData("conflateMs", "500")]
        [InlineData("heartbeatMs", "5000")]
        [InlineData("pt", "1234567890")]
        [InlineData("ct", "\"SUB_IMAGE\"")]
        [InlineData("segmentType", "\"SEG_START\"")]
        [InlineData("mc", "[{\"id\":\"1.167778679\"}]")]
        [InlineData("oc", "[{\"id\":\"1.167778679\"}]")]
        public async Task OnGetChangesChangeMessageIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"{property}\":{value}}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("id", "\"1.167778679\"")]
        [InlineData("img", "true")]
        [InlineData("tv", "1.2")]
        [InlineData("con", "true")]
        [InlineData("marketDefinition", "{\"bspMarket\":true}")]
        [InlineData("rc", "[{\"atb\":[[4.2,598.09]]}]")]
        public async Task OnGetChangesMarketChangeIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"mc\":[{{\"{property}\":{value}}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("bspMarket", "true")]
        [InlineData("turnInPlayEnabled", "true")]
        [InlineData("persistenceEnabled", "true")]
        [InlineData("marketBaseRate", "5")]
        [InlineData("eventId", "\"29664886\"")]
        [InlineData("eventTypeId", "\"1\"")]
        [InlineData("numberOfWinners", "1")]
        [InlineData("bettingType", "\"ODDS\"")]
        [InlineData("marketType", "\"MATCH_ODDS\"")]
        [InlineData("marketTime", "\"2020-02-09T17:00:41Z\"")]
        [InlineData("suspendTime", "\"2020-02-09T17:00:41Z\"")]
        [InlineData("bspReconciled", "false")]
        [InlineData("complete", "true")]
        [InlineData("inPlay", "true")]
        [InlineData("crossMatching", "true")]
        [InlineData("runnersVoidable", "false")]
        [InlineData("numberOfActiveRunners", "3")]
        [InlineData("betDelay", "5")]
        [InlineData("status", "\"OPEN\"")]
        [InlineData("runners", "[{\"status\":\"ACTIVE\"}]")]
        [InlineData("regulators", "[\"MR_INT\"]")]
        [InlineData("countryCode", "\"DE\"")]
        [InlineData("discountAllowed", "true")]
        [InlineData("timezone", "\"GMT\"")]
        [InlineData("openDate", "\"2020-02-09T17:00:41Z\"")]
        [InlineData("version", "3162427596")]
        [InlineData("venue", "\"Epsom\"")]
        [InlineData("settledTime", "\"2020-02-09T17:00:41Z\"")]
        [InlineData("eachWayDivisor", "1.2")]
        public async Task OnGetChangesMarketDefinitionIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"mc\":[{{\"marketDefinition\":{{\"{property}\":{value}}}}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("tv", "1.1")]
        [InlineData("spf", "1.1")]
        [InlineData("ltp", "1.1")]
        [InlineData("spn", "1.1")]
        [InlineData("hc", "1.1")]
        [InlineData("id", "5340398")]
        [InlineData("batb", "[[4.2,598.09]]")]
        [InlineData("spb", "[[4.2,598.09]]")]
        [InlineData("bdatl", "[[4.2,598.09]]")]
        [InlineData("trd", "[[4.2,598.09]]")]
        [InlineData("atb", "[[4.2,598.09]]")]
        [InlineData("spl", "[[4.2,598.09]]")]
        [InlineData("atl", "[[4.2,598.09]]")]
        [InlineData("batl", "[[4.2,598.09]]")]
        [InlineData("bdatb", "[[4.2,598.09]]")]
        public async Task OnGetChangesRunnerChangeIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"mc\":[{{\"rc\":[{{\"{property}\":{value}}}]}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("status", "\"ACTIVE\"")]
        [InlineData("sortPriority", "1")]
        [InlineData("removalDate", "\"2020-02-09T17:00:41Z\"")]
        [InlineData("id", "5340398")]
        [InlineData("hc", "1.1")]
        [InlineData("adjustmentFactor", "1.1")]
        [InlineData("bsp", "1.1")]
        public async Task OnGetChangesRunnerDefinitionIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"mc\":[{{\"marketDefinition\":{{\"runners\":[{{\"{property}\":{value}}}]}}}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("id", "\"1.167778679\"")]
        [InlineData("accountId", "1234567890")]
        [InlineData("closed", "true")]
        [InlineData("orc", "[{\"mb\":[[4.2,598.09]]}]")]
        public async Task OnGetChangesOrderChangeIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"oc\":[{{\"{property}\":{value}}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("mb", "[[4.2,598.09]]")]
        [InlineData("id", "1234567890")]
        [InlineData("hc", "1.1")]
        [InlineData("fullImage", "true")]
        [InlineData("ml", "[[4.2,598.09]]")]
        [InlineData("uo", "[{\"side\":\"B\"}]")]
        public async Task OnGetChangesOrderRunnerChangeIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"oc\":[{{\"orc\":[{{\"{property}\":{value}}}]}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("side", "\"B\"")]
        [InlineData("pt", "\"L\"")]
        [InlineData("ot", "\"L\"")]
        [InlineData("status", "\"EC\"")]
        [InlineData("sv", "1.1")]
        [InlineData("p", "1.1")]
        [InlineData("sc", "1.1")]
        [InlineData("rc", "\"REG\"")]
        [InlineData("s", "1.1")]
        [InlineData("pd", "1234567890")]
        [InlineData("rac", "\"REG\"")]
        [InlineData("md", "1234567890")]
        [InlineData("sl", "1.1")]
        [InlineData("avp", "1.1")]
        [InlineData("sm", "1.1")]
        [InlineData("id", "\"12345\"")]
        [InlineData("bsp", "1.1")]
        [InlineData("sr", "1.1")]
        public async Task OnGetChangesUnmatchedOrdersIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"oc\":[{{\"orc\":[{{\"uo\":[{{\"{property}\":{value}}}]}}]}}]}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Fact]
        public void OnSetArrivalTimeEpochMillisecondsIsSet()
        {
            var message = new ChangeMessage();
            var datetime = DateTime.Parse("2020-02-14 19:17:33.123", new NumberFormatInfo());
            message.SetArrivalTime(datetime);
            Assert.Equal(1581707853123, message.ArrivalTime);
        }

        [Fact]
        public async Task OnGetChangesArrivalTimeIsSet()
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"pt\":1581707853123}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.NotNull(message.ArrivalTime);
        }
    }
}
