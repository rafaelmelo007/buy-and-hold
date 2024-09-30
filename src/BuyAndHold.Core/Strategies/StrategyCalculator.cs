namespace BuyAndHold.Core.Strategies;
public class StrategyCalculator
{
    public static StrategyResults CalculateStrategyResults(
        string strategyName, Chart chart, WalletBalance wallet,
        IEnumerable<Order> orders,
        IEnumerable<FundInjection> fundInjections,
        IEnumerable<StrategyResultCandle> candles)
    {
        var totalInvested = wallet.InitialInWallet + fundInjections.Sum(x => x.Amount);
        var totalReceived = wallet.InWallet + wallet.InCustody;
        var totalProfit = wallet.CalculateProfit();
        var profitVariation = totalInvested > 0 ? (totalReceived / totalInvested - 1) * 100 : default;

        var results = new StrategyResults
        {
            SymbolName = chart.Symbol,
            BeginDate = chart.Begin,
            EndDate = chart.End,
            StrategyName = strategyName,
            TotalInvested = totalInvested,
            TotalReceived = totalReceived,
            TotalProfit = totalProfit * 100,
            AveragePrice = wallet.Positions.Any() ?
                            wallet.Positions.Values.Sum(x => x.AveragePrice * x.Quantity) / wallet.Positions.Values.Sum(x => x.Quantity) : default,
            CurrentPrice = chart.Candles.Values.Last().Close,
            ProfitVariation = profitVariation,
            TotalTrades = wallet.Orders.Count,
            TotalInAccount = wallet.InWallet,
            TotalInCustody = wallet.InCustody,
            TotalQuantityInCustody = wallet.Orders.Sum(x => x.Type == OrderType.Buy ? x.Quantity : -x.Quantity),
            Candles = candles,
            ResultYears = wallet.ResultYears
        };

        var tradeDates = orders.Select(t => t.Date).Distinct().OrderBy(d => d).ToList();
        if (tradeDates.Count > 1)
        {
            var holdingPeriod = tradeDates.Last().ToDateTime(new TimeOnly()) - tradeDates.First().ToDateTime(new TimeOnly());
            results.TotalDaysHolding = (int)holdingPeriod.TotalDays;
            results.TotalMonthsHolding = (int)(holdingPeriod.TotalDays / 30);
            results.TotalYearsHolding = (int)(holdingPeriod.TotalDays / 365);
        }

        return results;
    }
}