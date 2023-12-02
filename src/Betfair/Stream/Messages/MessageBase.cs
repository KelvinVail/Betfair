namespace Betfair.Stream.Messages;

internal abstract class MessageBase
{
    protected MessageBase(string operation, int id)
    {
        Op = operation;
        Id = id;
    }

    public string Op { get; }

    public int Id { get; }
}
