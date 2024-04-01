namespace Betfair.Stream.Responses;

public class StatusMessage
{
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "connectionClosed")]
    public bool IsClosed { get; set; }
}
