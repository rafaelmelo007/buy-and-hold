namespace BuyAndHold.Core.Strategies;
public abstract class BaseMonthlyStrategy : IInvestStrategy
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract double MonthlyInvestment { get; }
    public abstract string[] UsingSymbols { get; }

    public abstract StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet);

    protected Order? ProcessBuy(Candle candle, WalletBalance balance, double percentage, string opportunityInfo)
    {
        // Implement the specific buy logic for BrentHighLowStrategy
        var availableFunds = balance.InWallet * percentage;
        var quantityToBuy = (int)(availableFunds / candle.Open);
        if (quantityToBuy > 0)
        {
            var order = new Order
            {
                Symbol = candle.Symbol,
                Quantity = quantityToBuy,
                UnitPrice = candle.Open,
                Type = OrderType.Buy,
                Date = candle.Date,
                OpportunityInfo = opportunityInfo
            };
            balance.AppendOrder(order);
            return order;
        }
        return null;
    }

    protected Order? ProcessSell(Candle candle, WalletBalance balance, double percentage, string opportunityInfo)
    {
        // Implement the specific sell logic for BrentHighLowStrategy
        var availableFunds = balance.InCustody * percentage;
        var quantityToSell = (int)(availableFunds / candle.Open);
        var currentPosition = balance.Positions.Values.FirstOrDefault(p => p.Symbol == candle.Symbol);

        if (currentPosition != null && quantityToSell > currentPosition.Quantity)
        {
            quantityToSell = currentPosition.Quantity;
        }

        if (quantityToSell > 0 && currentPosition != null)
        {
            var order = new Order
            {
                Symbol = candle.Symbol,
                Quantity = quantityToSell,
                UnitPrice = candle.Open,
                Type = OrderType.Sell,
                Date = candle.Date,
                OpportunityInfo = opportunityInfo
            };
            balance.AppendOrder(order);
            return order;
        }
        return null;
    }

    protected StrategyResults ExecuteStrategy(
        string strategyName, Chart chart, WalletBalance wallet,
        Func<Candle, WalletBalance, Order?> executeLogic,
        int? dayOfInvestment = null)
    {
        var startDate = chart.Candles.Values.Min(c => c.Date);
        var endDate = DateOnly.MaxValue;
        var candles = chart.Candles.Values
            .Where(c => c.Date >= startDate && c.Date <= endDate)
            .OrderBy(c => c.Date)
            .ToList();

        var processedMonths = new HashSet<string>();
        var strategyCandles = new List<StrategyResultCandle>();
        int yearPreviousCandle = candles.FirstOrDefault().Date.Year;

        foreach (var candle in candles)
        {
            if (yearPreviousCandle != candle.Date.Year)
            {
                wallet.StoreYearMetrics(yearPreviousCandle);
            }

            var monthKey = $"{candle.Date.Year}-{candle.Date.Month}";
            Order? orderExecuted = null;

            // Determine whether to process this candle based on the day of the month
            bool shouldProcess = !processedMonths.Contains(monthKey) && IsTradingDay(candle);

            if (shouldProcess)
            {
                if (dayOfInvestment.HasValue)
                {
                    // Only process if the current candle date is greater than or equals to dayOfInvestment
                    shouldProcess = candle.Date.Day >= dayOfInvestment.Value;
                }

                if (shouldProcess)
                {
                    wallet.AddFunds(candle.Date, MonthlyInvestment);
                    processedMonths.Add(monthKey);

                    orderExecuted = executeLogic(candle, wallet);
                }
            }

            var fundAdded = wallet.FundInjections.FirstOrDefault(f => f.Date == candle.Date);
            strategyCandles.Add(new StrategyResultCandle(chart.Symbol, wallet, candle, orderExecuted, fundAdded));
            yearPreviousCandle = candle.Date.Year;
        }

        return CalculateStrategyResults(strategyName, chart, wallet, strategyCandles);
    }


    protected bool IsTradingDay(Candle candle)
    {
        return candle.Date.DayOfWeek != DayOfWeek.Saturday && candle.Date.DayOfWeek != DayOfWeek.Sunday;
    }

    protected StrategyResults CalculateStrategyResults(string strategyName, Chart chart, WalletBalance balance, IEnumerable<StrategyResultCandle> candles)
    {
        return StrategyCalculator.CalculateStrategyResults(strategyName, chart, balance, balance.Orders, balance.FundInjections, candles);
    }

    public abstract string GetSettingsJson();
    public abstract bool LoadSettingsFromJson(string content);
}
