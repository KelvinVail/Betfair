﻿namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class OrderRunnerChangeStub : OrderRunnerChange
    {
        public OrderRunnerChangeStub(long selectionId = 12345)
        {
            this.SelectionId = selectionId;
        }

        public OrderRunnerChangeStub WithMatchedBack(double? price, double? size)
        {
            this.MatchedBacks ??= new List<List<double?>>();
            this.MatchedBacks.Add(new List<double?> { price, size });
            return this;
        }

        public OrderRunnerChangeStub WithMatchedLay(double? price, double? size)
        {
            this.MatchedLays ??= new List<List<double?>>();
            this.MatchedLays.Add(new List<double?> { price, size });
            return this;
        }

        public OrderRunnerChangeStub WithUnmatchedLay(double? price, double? size)
        {
            this.UnmatchedOrders ??= new List<UnmatchedOrder>();
            var uo = new UnmatchedOrder
            {
                Side = "L",
                Price = price,
                SizeRemaining = size,
            };
            this.UnmatchedOrders.Add(uo);
            return this;
        }

        public OrderRunnerChangeStub WithUnmatchedBack(double? price, double? size)
        {
            this.UnmatchedOrders ??= new List<UnmatchedOrder>();
            var uo = new UnmatchedOrder
            {
                Side = "B",
                Price = price,
                SizeRemaining = size,
            };
            this.UnmatchedOrders.Add(uo);
            return this;
        }
    }
}
