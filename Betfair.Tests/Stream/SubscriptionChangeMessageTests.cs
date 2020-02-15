namespace Betfair.Tests.Stream
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Xunit;

    public sealed class SubscriptionChangeMessageTests : SubscriptionTests
    {
        [Theory]
        [InlineData("conflateMs", "500")]
        [InlineData("heartbeatMs", "5000")]
        [InlineData("pt", "1234567890")]
        [InlineData("ct", "\"SUB_IMAGE\"")]
        [InlineData("segmentType", "\"SEG_START\"")]
        public async Task OnGetChangesChangeMessageIsDeserialized(string property, string value)
        {
            var receivedLine = $"{{\"op\":\"mcm\",\"{property}\":{value}}}";
            var message = await this.SendLineAsync(receivedLine);
            Assert.StartsWith(receivedLine.Remove(receivedLine.Length - 1), message.ToJson(), StringComparison.CurrentCulture);
        }

        [Fact]
        public void OnSetArrivalTimeEpochMillisecondsIsSet()
        {
            var message = new ResponseMessage();
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
