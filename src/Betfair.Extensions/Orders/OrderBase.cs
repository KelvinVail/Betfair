using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Extensions.Orders;

public abstract class OrderBase(long selectionId, Side side)
{
    public long SelectionId { get; } = selectionId;

    public Side Side { get; } = side;

    internal abstract PlaceInstruction ToInstruction();
}