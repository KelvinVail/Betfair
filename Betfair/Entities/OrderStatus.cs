namespace Betfair.Entities
{
    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Utils;

    /// <summary>
    /// The order status.
    /// </summary>
    public class OrderStatus
    {
        /// <summary>
        /// Gets or sets the selection id.
        /// </summary>
        public long SelectionId { get; protected set; }

        /// <summary>
        /// Gets or sets the bet id.
        /// </summary>
        public string BetId { get; protected set; }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        public Side Side { get; protected set; }

        /// <summary>
        /// Gets or sets the requested price.
        /// </summary>
        public double RequestedPrice { get; protected set; }

        /// <summary>
        /// Gets or sets the requested size.
        /// </summary>
        public double RequestedSize { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this order is fully matched.
        /// </summary>
        public bool IsFullyMatched { get; protected set; }

        /// <summary>
        /// Gets or sets the average price matched.
        /// </summary>
        public double AveragePriceMatched { get; protected set; }

        /// <summary>
        /// Gets or sets the size matched.
        /// </summary>
        public double SizeMatched { get; set; }

        /// <summary>
        /// Gets the matched liability.
        /// </summary>
        public double MatchedLiability => ProfitHelper.Liability(this.Side, this.AveragePriceMatched, this.SizeMatched);

        /// <summary>
        /// The profit or loss if this runner wins.
        /// </summary>
        public double MatchedIfWin => ProfitHelper.IfWin(this.Side, this.AveragePriceMatched, this.SizeMatched);

        /// <summary>
        /// The profit or loss if this runner loses.
        /// </summary>
        public double MatchedIfLose => ProfitHelper.IfLose(this.Side, this.SizeMatched);

        /// <summary>
        /// The implied odds.
        /// </summary>
        public double MatchedImpliedOdds => PriceHelper.ImpliedOdds(this.AveragePriceMatched);
    }
}
