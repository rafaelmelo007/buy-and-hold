namespace BuyAndHold.Core.Strategies;
public class IbovHighLowStrategy : BaseMonthlyStrategy
{
    public class IbovHighLowSettings
    {
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double MonthlyInvestment { get; set; } = 1000;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowLimitPercent { get; set; } = 10;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowerLimitPercent { get; set; } = 25;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowBuyPercent { get; set; } = 0.25;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double LowerBuyPercent { get; set; } = 1.0;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double HighLimitPercent { get; set; } = 25;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double HighSellPercent { get; set; } = 0.25;

        [JsonConverter(typeof(StringToIntConverter))]
        public int MonthsToLookBack { get; set; } = 9;
        [JsonConverter(typeof(StringToIntConverter))]
        public int DayOfTheMonthToInvest { get; set; } = 1;
    }

    private Dictionary<DateOnly, double> _ibovHistoricalPrices = new Dictionary<DateOnly, double>();
    public IbovHighLowSettings Settings { get; set; } = new IbovHighLowSettings();

    public IbovHighLowStrategy() { }

    public override string Name => "IBOV High Low Strategy";
    public override string Description => $"This strategy invests based on IBOV percentage changes from {Settings.MonthsToLookBack} months ago. Invests {Settings.LowBuyPercent * 100}% of the account if IBOV drops by 10%, {Settings.LowerBuyPercent * 100}% if it drops by 25%. If IBOV is between {Settings.LowLimitPercent * 100}% and {Settings.HighLimitPercent * 100}%, the monthly investment is added to the balance. Sells {Settings.HighSellPercent * 100}% of holdings if IBOV rises above 20%.";
    public override double MonthlyInvestment => Settings.MonthlyInvestment;
    public override string[] UsingSymbols => ["^BVSP"];

    public override StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet)
    {
        var mainChart = charts[symbol];
        var ibovChart = charts["^BVSP"];
        LoadIbovHistoricalPrices(ibovChart);

        return ExecuteStrategy(strategyName, mainChart, wallet, ExecuteLogic, Settings.DayOfTheMonthToInvest);
    }

    private Order? ExecuteLogic(Candle candle, WalletBalance wallet)
    {
        // Determine the price 6 months ago
        var lookBackDate = candle.Date.AddMonths(-Settings.MonthsToLookBack);
        if (_ibovHistoricalPrices.TryGetValue(lookBackDate, out var historicalPrice) &&
            _ibovHistoricalPrices.TryGetValue(candle.Date, out var currentPrice))
        {
            var priceChange = ((currentPrice - historicalPrice) / historicalPrice) * 100;

            if (priceChange <= -Settings.LowLimitPercent)
            {
                var investPercent = priceChange <= -Settings.LowerLimitPercent ? Settings.LowerBuyPercent : Settings.LowBuyPercent;
                var opportunityInfo = $"IBOV has dropped -{priceChange:#.00}% in the last {Settings.MonthsToLookBack} month(s). In scenarios like this, it is recommended to buy {investPercent * 100}% of wallet in {candle.Symbol}.";
                return ProcessBuy(candle, wallet, investPercent, opportunityInfo);
            }
            else if (priceChange > Settings.HighLimitPercent)
            {
                var opportunityInfo = $"IBOV has rised {priceChange:#.00}% in the last {Settings.MonthsToLookBack} month(s). In scenarios like this, it is recommended to sell {Settings.HighSellPercent * 100}% of {candle.Symbol} in custody.";
                return ProcessSell(candle, wallet, Settings.HighSellPercent, opportunityInfo);
            }
            // No else case needed; remaining funds are already added monthly
        }

        return null;
    }

    private void LoadIbovHistoricalPrices(Chart ibovChart)
    {
        _ibovHistoricalPrices = ibovChart.Candles.Values.ToDictionary(c => c.Date, c => c.Open);
    }

    public override string GetSettingsJson()
    {
        return JsonSerializer.Serialize(Settings);
    }

    public override bool LoadSettingsFromJson(string content)
    {
        var settings = JsonSerializer.Deserialize<IbovHighLowSettings>(content);
        if (settings != null)
        {
            Settings = settings;
            return true;
        }
        return false;
    }
}
