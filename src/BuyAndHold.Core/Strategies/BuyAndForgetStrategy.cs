namespace BuyAndHold.Core.Strategies;
public class BuyAndForgetStrategy : BaseMonthlyStrategy
{
    public class BuyAndForgetSettings
    {
        [JsonConverter(typeof(StringToIntConverter))]
        public int DayOfMonthToInvest { get; set; } = 1;

        [JsonConverter(typeof(StringToDecimalConverter))]
        public double MonthlyInvestment { get; set; } = 1000;
    }

    public BuyAndForgetSettings Settings { get; private set; } = new BuyAndForgetSettings();

    public override string Name => "Buy and Forget";

    public override string Description =>
        $"In this strategy, the investor will invest a fixed amount of money on the " +
        $"{Settings.DayOfMonthToInvest} of each month in one stock market paper over time. " +
        $"Parameters: MonthlyInvestment (default: {Settings.MonthlyInvestment:C}).";

    public override double MonthlyInvestment => Settings.MonthlyInvestment;
    public override string[] UsingSymbols => [];

    public override StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet)
    {
        var chart = charts[symbol];

        return ExecuteStrategy(strategyName, chart, wallet, (candle, wallet) =>
        {
            var opportunityInfo = $"In the buy and forget strategy, it is recommended to buy ${Settings.MonthlyInvestment} in {symbol}.";
            var order = ProcessBuy(candle, wallet, 1, opportunityInfo);
            return order;
        }, Settings.DayOfMonthToInvest);
    }

    public override string GetSettingsJson()
    {
        return JsonSerializer.Serialize(Settings);
    }

    public override bool LoadSettingsFromJson(string content)
    {
        var settings = JsonSerializer.Deserialize<BuyAndForgetSettings>(content);
        if (settings != null)
        {
            Settings = settings;
            return true;
        }
        return false;
    }
}