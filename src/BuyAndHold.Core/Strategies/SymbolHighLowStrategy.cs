namespace BuyAndHold.Core.Strategies;

public class SymbolHighLowStrategy : BaseMonthlyStrategy
{
    public class SymbolHighLowSettings
    {
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double MonthlyInvestment { get; set; } = 1000;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowBuyPercent { get; set; } = 0.25;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowerBuyPercent { get; set; } = 0.5;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double HighSellPercent { get; set; } = 0.5;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowLimitPercent { get; set; } = 10;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowerLimitPercent { get; set; } = 30;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double HighLimitPercent { get; set; } = 40;

        [JsonConverter(typeof(StringToIntConverter))]
        public int MonthsToLookBack { get; set; } = 12;
        [JsonConverter(typeof(StringToIntConverter))]
        public int DayOfTheMonthToInvest { get; set; } = 1;
    }

    private Dictionary<DateOnly, double> _symbolHistoricalPrices = new Dictionary<DateOnly, double>();
    public SymbolHighLowSettings Settings { get; set; } = new SymbolHighLowSettings();

    public override string Name => $"Symbol High Low Strategy";
    public override string Description => $"This strategy invests based on symbol percentage changes from {Settings.MonthsToLookBack} months ago. Invests {Settings.LowBuyPercent * 100}% of the account if symbol drops by 10%, {Settings.LowerBuyPercent * 100}% if it drops by 25%. If symbol is between {Settings.LowLimitPercent * 100}% and {Settings.HighLimitPercent * 100}%, the monthly investment is added to the balance. Sells {Settings.HighSellPercent * 100}% of holdings if symbol rises above 20%.";
    public override double MonthlyInvestment => Settings.MonthlyInvestment;
    public override string[] UsingSymbols => [];

    public override StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet)
    {
        var chart = charts[symbol];
        LoadSymbolHistoricalPrices(chart);

        return ExecuteStrategy(strategyName, chart, wallet, ExecuteLogic, Settings.DayOfTheMonthToInvest);
    }

    private Order? ExecuteLogic(Candle candle, WalletBalance wallet)
    {
        // Determine the price X months ago
        var lookBackDate = candle.Date.AddMonths(-Settings.MonthsToLookBack);
        if (_symbolHistoricalPrices.TryGetValue(lookBackDate, out var historicalPrice) &&
            _symbolHistoricalPrices.TryGetValue(candle.Date, out var currentPrice))
        {
            var priceChange = ((currentPrice - historicalPrice) / historicalPrice) * 100;

            if (priceChange <= -Settings.LowLimitPercent)
            {
                var investPercent = priceChange <= -Settings.LowerLimitPercent ? Settings.LowerBuyPercent : Settings.LowBuyPercent;
                var opportunityInfo = $"{candle.Symbol} has dropped -{priceChange:#.00}% in the last {Settings.MonthsToLookBack} month(s). In scenarios like this, it is recommended to buy {investPercent}% of wallet in {candle.Symbol}. lookBackDate:{lookBackDate:yyyy-MM-dd}; historicalPrice: {historicalPrice:#.00}; candleDate: {candle.Date:yyyy-MM-dd}; currentPrice: {currentPrice:#.00}";
                return ProcessBuy(candle, wallet, investPercent, opportunityInfo);
            }
            else if (priceChange > Settings.HighLimitPercent)
            {
                var opportunityInfo = $"{candle.Symbol} has rised {priceChange:#.00}% in the last {Settings.MonthsToLookBack} month(s). In scenarios like this, it is recommended to sell {Settings.HighSellPercent * 100}% of {candle.Symbol} in custody.";
                return ProcessSell(candle, wallet, Settings.HighSellPercent, opportunityInfo);
            }
            // No else case needed; remaining funds are already added monthly
        }

        return null;
    }

    private void LoadSymbolHistoricalPrices(Chart symbolChart)
    {
        _symbolHistoricalPrices = symbolChart.Candles.Values.ToDictionary(c => c.Date, c => c.Open);
    }

    public override string GetSettingsJson()
    {
        return JsonSerializer.Serialize(Settings);
    }

    public override bool LoadSettingsFromJson(string content)
    {
        var settings = JsonSerializer.Deserialize<SymbolHighLowSettings>(content);
        if (settings != null)
        {
            Settings = settings;
            return true;
        }
        return false;
    }
}
