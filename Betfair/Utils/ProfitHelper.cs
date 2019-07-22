namespace Betfair.Utils
{
    using System;

    using Betfair.Services.BetfairApi.Enums;

    /// <summary>
    /// Profit helper methods.
    /// </summary>
    public class ProfitHelper
    {
        /// <summary>
        /// The profit if the selection wins.
        /// </summary>
        /// <param name="side">
        /// The side.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double IfWin(Side side, double price, double size)
        {
            return side == Side.BACK ? Math.Round((size * price) - size) : Math.Round(-((size * price) - size));
        }

        /// <summary>
        /// The profit if the selection loses.
        /// </summary>
        /// <param name="side">
        /// The side.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double IfLose(Side side, double size)
        {
            return side == Side.BACK ? -size : size;
        }

        /// <summary>
        /// The liability.
        /// </summary>
        /// <param name="side">
        /// The side.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Liability(Side side, double price, double size)
        {
            var ifWin = IfWin(side, price, size);
            var ifLose = IfLose(side, size);
            return Math.Min(ifWin, ifLose);
        }
    }
}
