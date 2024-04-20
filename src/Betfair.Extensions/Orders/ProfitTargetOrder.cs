using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Extensions.Orders;

/// <summary>
/// Place a bet specifying your target profit or liability,
/// instead of the backers stake ('size').
/// Profit target bets automatically calculate the size of bet needed to
/// achieve the specified profit target.
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="price">The price at which to place the bet.</param>
/// <param name="profitTarget">From the backers perspective, the payout requested
/// minus the calculated size at which this LimitOrder is to be placed.</param>
/// <param name="persistenceType">What to do with the bet at turn-in-play.</param>
public class ProfitTargetOrder(
    long selectionId,
    Side side,
    double price,
    double profitTarget,
    PersistenceType? persistenceType = null)
    : OrderBase(selectionId, side)
{
    public double Price { get; } = price;

    public double ProfitTarget { get; } = profitTarget;

    public PersistenceType PersistenceType { get; } = persistenceType ?? PersistenceType.Lapse;

    internal override PlaceInstruction ToInstruction() =>
        new ()
        {
            SelectionId = SelectionId,
            Side = Side,
            LimitOrder = new Api.Requests.Orders.LimitOrder
            {
                Price = Price,
                PersistenceType = PersistenceType,
                BetTargetType = "BACKERS_PROFIT",
            },
        };
}