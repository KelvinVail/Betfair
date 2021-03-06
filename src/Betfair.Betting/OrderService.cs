﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Betfair.Exchange.Interfaces;

namespace Betfair.Betting
{
    public sealed class OrderService : IOrderService
    {
        private readonly IExchangeService _exchange;

        public OrderService(IExchangeService exchange)
        {
            _exchange = ValidateExchangeService(exchange);
        }

        public async Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            ValidateMarketId(marketId);
            ValidateStrategyRef(strategyRef);
            var validOrders = ValidateOrders(orders);
            await PlaceOrders(marketId, validOrders, strategyRef);
            await PlaceBelowMinimumOrders(marketId, validOrders, strategyRef);
        }

        public async Task Cancel(string marketId, List<string> betIds)
        {
            if (betIds.Any())
                await _exchange.SendAsync<PlaceReport>("betting", "cancelOrders", CancelParams(marketId, betIds));
        }

        public async Task CancelAll(string marketId)
        {
            await _exchange.SendAsync<dynamic>("betting", "cancelOrders", $"{{\"marketId\":\"{marketId}\"}}");
        }

        private static IExchangeService ValidateExchangeService(IExchangeService exchange)
        {
            if (exchange is null)
                throw new ArgumentNullException(nameof(exchange), "ExchangeService should not be null.");
            return exchange;
        }

        private static void ValidateMarketId(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
        }

        private static void ValidateStrategyRef(string strategyRef)
        {
            if (string.IsNullOrEmpty(strategyRef)) return;
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
        }

        private static List<LimitOrder> ValidateOrders(List<LimitOrder> orders)
        {
            if (!orders.Any()) throw new InvalidOperationException("Does not contain any orders.");
            return orders.Where(o => o.Size > 0 && o.ToInstruction() != null).ToList();
        }

        private static string Instructions(List<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.ForEach(i => instructions += i.ToInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static string Params(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            return $"{{\"marketId\":\"{marketId}\",{Reference(strategyRef)}\"instructions\":[{Instructions(orders)}]}}";
        }

        private static string BelowMinimumCancelParams(string marketId, IEnumerable<LimitOrder> orders)
        {
            return $"{{\"marketId\":\"{marketId}\",\"instructions\":[{BelowMinimumCancelInstructions(orders)}]}}";
        }

        private static string BelowMinimumCancelInstructions(IEnumerable<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumCancelInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static string BelowMinimumReplaceParams(string marketId, IEnumerable<LimitOrder> orders, string strategyRef)
        {
            return $"{{\"marketId\":\"{marketId}\",{Reference(strategyRef)}\"instructions\":[{BelowMinimumReplaceInstructions(orders)}]}}";
        }

        private static string BelowMinimumReplaceInstructions(IEnumerable<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumReplaceInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static void UpdateOrders(PlaceReport reports, List<LimitOrder> orders)
        {
            if (reports is null) return;
            orders.ForEach(o => o.AddReports(reports.InstructionReports));
        }

        private static void UpdateOrders(ReplaceReport reports, List<LimitOrder> orders)
        {
            if (reports is null) return;
            var r = reports.InstructionReports.Select(i => i.PlaceInstructionReport).ToList();
            orders.ForEach(o => o.AddReports(r));
        }

        private static string Reference(string strategyRef)
        {
            return strategyRef is null ? null : $"\"customerStrategyRef\":\"{strategyRef}\",";
        }

        private static string CancelParams(string marketId, List<string> betIds)
        {
            return $"{{\"marketId\":\"{marketId}\",\"instructions\":[{CancelInstructions(betIds)}]}}";
        }

        private static string CancelInstructions(List<string> betIds)
        {
            var cancelInstructions = string.Empty;
            betIds.ForEach(i => cancelInstructions += $"{{\"betId\":\"{i}\"}}" + ",");
            return cancelInstructions.Remove(cancelInstructions.Length - 1, 1);
        }

        private async Task PlaceOrders(string marketId, List<LimitOrder> orders, string strategyRef)
        {
            var report = await _exchange.SendAsync<PlaceReport>("betting", "placeOrders", Params(marketId, orders, strategyRef));
            UpdateOrders(report, orders);
        }

        private async Task PlaceBelowMinimumOrders(string marketId, List<LimitOrder> orders, string strategyRef)
        {
            if (orders.Any(o => o.BelowMinimumStake))
            {
                await _exchange.SendAsync<PlaceReport>("betting", "cancelOrders", BelowMinimumCancelParams(marketId, orders));
                var replaceReport =
                    await _exchange.SendAsync<ReplaceReport>("betting", "replaceOrders", BelowMinimumReplaceParams(marketId, orders, strategyRef));
                UpdateOrders(replaceReport, orders);
            }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        [DataContract]
        private sealed class PlaceReport
        {
            [DataMember(Name = "instructionReports", EmitDefaultValue = false)]
            internal List<InstructionReport> InstructionReports { get; set; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        [DataContract]
        private sealed class ReplaceReport
        {
            [DataMember(Name = "instructionReports", EmitDefaultValue = false)]
            public List<ReplaceInstructionReport> InstructionReports { get; set; }
        }
    }
}
