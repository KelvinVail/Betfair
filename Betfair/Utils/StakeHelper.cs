namespace Betfair.Utils
{
    using System;

    /// <summary>
    /// Static functions to help with common Betfair stake calculations.
    /// </summary>
    public class StakeHelper
    {
        /// <summary>
        /// Calculates the minimum valid stake for a given price.
        /// </summary>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <returns>
        /// The minimum valid stake. <see cref="double"/>.
        /// </returns>
        public static double MinimumStake(double price)
        {
            var result = Math.Ceiling(10 / price * 100) / 100;
            return result < 2 ? result : 2.0;
        }

        /// <summary>
        /// Is this stake above the minimum stake for this price?
        /// </summary>
        /// <param name="stake">
        /// The stake to be evaluated.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <returns>
        /// Returns true if the stake is above the minimum false if it is not. <see cref="bool"/>.
        /// </returns>
        public static bool IsStakeAboveMinimum(double stake, double price)
        {
            var minStake = MinimumStake(price);
            return stake >= minStake;
        }

        /// <summary>
        /// Is this stake below the minimum stake for this price?
        /// </summary>
        /// <param name="stake">
        /// The stake to be evaluated.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <returns>
        /// Returns true if the stake is below the minimum false if it is not. <see cref="bool"/>.
        /// </returns>
        public static bool IsStakeBelowMinimum(double stake, double price)
        {
            var minStake = MinimumStake(price);
            return stake < minStake;
        }
    }
}
