namespace BuyAndHold.Core.Strategies;
public class PriceMovementStrategy : IInvestStrategy
{
    public class PriceMovementSettings
    {
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double InvestmentAmount { get; set; } = 1000;
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double FallThreshold { get; set; } = 0.05; // 5% fall
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double RiseThreshold { get; set; } = 0.10; // 10% rise
        [JsonConverter(typeof(StringToIntConverter))]
        public int FallDays { get; set; } = 3; // Number of days to check for falls
        [JsonConverter(typeof(StringToIntConverter))]
        public int RiseDays { get; set; } = 6; // Number of days to check for rises
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double InvestAmountOnFall { get; set; } = 0.5; // 50% of InvestmentAmount
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double SellAmountOnRise { get; set; } = 0.25; // 25% of holdings
    }

    public PriceMovementSettings Settings { get; set; } = new PriceMovementSettings();

    public PriceMovementStrategy()
    {
    }

    public string Name => "Price Movement Strategy";
    public string Description => $"This strategy invests based on recent price movements. Invests {Settings.InvestAmountOnFall * 100}% of {Settings.InvestmentAmount} if the price falls more than {Settings.FallThreshold * 100}% over {Settings.FallDays} days. Sells {Settings.SellAmountOnRise * 100}% of holdings if the price rises more than {Settings.RiseThreshold * 100}% over {Settings.RiseDays} days.";
    public string[] UsingSymbols => [];

    public StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet)
    {
        var chart = charts[symbol];
        var candles = chart.Candles.Values
            .OrderBy(c => c.Date)
            .ToList();

        var processedDates = new HashSet<DateOnly>();

        var monthsProcessed = new List<string>();

        var strategyCandles = new List<StrategyResultCandle>();
        foreach (var candle in candles)
        {
            if (processedDates.Contains(candle.Date))
                continue;

            processedDates.Add(candle.Date);

            Order? orderExecuted = null;

            // Add the monthly investment to the balance
            var monthKey = $"{candle.Date.Year}__{candle.Date.Month}";
            if (!monthKey.Contains(monthKey))
            {
                wallet.AddFunds(candle.Date, Settings.InvestmentAmount);
                monthsProcessed.Add(monthKey);
            }


            var daysTake = candle.Date.AddDays(-15);
            var fallPrices = candles
                .Where(c => c.Date >= daysTake && c.Date <= candle.Date)
                .OrderByDescending(c => c.Date)
                .Take(Settings.FallDays)
                .ToList()
                .OrderBy(x => x.Date)
                .ToList();

            // Check if we have exactly X days of data
            if (fallPrices.Count == Settings.FallDays &&
                fallPrices.ArePricesFalling() &&
                fallPrices.AreCandleBodiesValid())
            {
                // Calculate the fall amount using the extension method
                var fallAmount = fallPrices.CalculateMovePercentage();

                // If the fall amount meets the threshold, execute the buy order
                if (fallAmount <= -Settings.FallThreshold)
                {
                    var opportunityInfo = $"The price of {symbol} has fallen {Settings.FallThreshold} consecutive times. We recommend it to buy {Settings.InvestAmountOnFall * 100:#.00}%";
                    orderExecuted = ProcessBuy(candle, wallet, Settings.InvestAmountOnFall, opportunityInfo);
                }
            }

            // Select the range of candles for the last Y days, using Take to handle weekends/holidays
            daysTake = candle.Date.AddDays(-15);
            var risePrices = candles
                .Where(c => c.Date >= daysTake && c.Date <= candle.Date)
                .OrderByDescending(c => c.Date)
                .Take(Settings.RiseDays)
                .ToList()
                .OrderBy(x => x.Date)
                .ToList();

            // Check if we have exactly Y days of data
            if (risePrices.Count == Settings.RiseDays && risePrices.ArePricesRising())
            {
                // Calculate the rise percentage using the extension method
                var risePercentage = risePrices.CalculateMovePercentage();

                // If the rise percentage meets the threshold, execute the sell order
                if (risePercentage >= Settings.RiseThreshold)
                {
                    var opportunityInfo = $"The price of {symbol} has rised {risePercentage * 100:00}. We recommend to sell {Settings.SellAmountOnRise * 100:#.00}% in custody.";
                    orderExecuted = ProcessSell(candle, wallet, Settings.SellAmountOnRise, opportunityInfo);
                }
            }

            var fundAdded = wallet.FundInjections.FirstOrDefault(x => x.Date == candle.Date);

            strategyCandles.Add(new StrategyResultCandle(chart.Symbol, wallet, candle, orderExecuted, fundAdded));
        }

        return CalculateStrategyResults(strategyName, chart, wallet, strategyCandles);
    }

    public string? GetSettingsJson()
    {
        return Settings.ToJson();
    }

    public bool LoadSettingsFromJson(string content)
    {
        Settings = JsonSerializer.Deserialize<PriceMovementSettings>(content);
        return true;
    }

    #region Helper Methods

    private Order? ProcessBuy(Candle candle, WalletBalance balance, double percentage, string opportunityInfo)
    {
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
        return default;
    }

    private Order? ProcessSell(Candle candle, WalletBalance balance, double percentage, string opportunityInfo)
    {
        var availableFunds = balance.InCustody * percentage;
        var quantityToSell = (int)(availableFunds / candle.Open);
        if (quantityToSell > 0)
        {
            if (quantityToSell > balance.Positions.First().Value.Quantity)
            {
                quantityToSell = balance.Positions.First().Value.Quantity;
            }
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
        return default;
    }

    private StrategyResults CalculateStrategyResults(string strategyName, Chart chart, WalletBalance balance, IEnumerable<StrategyResultCandle> candles)
    {
        return StrategyCalculator.CalculateStrategyResults(strategyName, chart, balance, balance.Orders, balance.FundInjections, candles);
    }

    #endregion
}
