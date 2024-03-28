namespace Betfair.Stream.Responses.Deserializers;

public class ConnectionMessage
{
    private ConnectionMessage(string id)
    {
        ConnectionId = id;
    }

    public string ConnectionId { get; }

    // public static ConnectionMessage? Create(ReadOnlySequence<byte> data)
    // {
    //     var reader = new Utf8JsonReader(data);
    //     while (reader.Read())
    //     {
    //         if (reader.TokenType != JsonTokenType.PropertyName) continue;
    //         if (!reader.ValueTextEquals("op")) continue;
    //         reader.Read();
    //         if (reader.GetString() == "connection")
    //         {
    //             return new ConnectionMessage();
    //         }
    //
    //         return null;
    //     }
    // }
}
