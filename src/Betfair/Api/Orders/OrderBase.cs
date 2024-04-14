using Betfair.Api.Requests.OrderDtos;
using Betfair.Core;

namespace Betfair.Api.Orders;

public abstract class OrderBase(long selectionId, Side side)
{
    public long SelectionId { get; } = selectionId;

    public string Side { get; } = side.Value;

    internal abstract PlaceInstruction ToInstruction();
}