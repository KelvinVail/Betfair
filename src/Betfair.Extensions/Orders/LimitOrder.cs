using Betfair.Api.Requests.OrderDtos;
using Betfair.Core;

namespace Betfair.Extensions.Orders;

/// <summary>
/// A simple exchange bet for immediate execution.
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="price">The price at which to place the bet.</param>
/// <param name="size">The size of the bet.</param>
/// <param name="persistenceType">What to do with the bet at turn-in-play. Optional: default is LAPSE.</param>
public class LimitOrder(
    long selectionId,
    Side side,
    double price,
    double size,
    PersistenceType? persistenceType = null)
    : OrderBase(selectionId, side)
{
    public double Price { get; } = price;

    public double Size { get; } = size;

    public string PersistenceType { get; } = persistenceType?.Value ?? Core.PersistenceType.Lapse.Value;

    internal override PlaceInstruction ToInstruction() =>
        new ()
        {
            SelectionId = SelectionId,
            Side = Side,
            LimitOrder = new Api.Requests.OrderDtos.LimitOrder
            {
                Price = Price,
                Size = Size,
                PersistenceType = PersistenceType,
            },
        };
}