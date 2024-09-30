namespace BuyAndHold.Core.Strategies;
public class BrentHighLowStrategy : BaseMonthlyStrategy
{
    public class BrentHighLowSettings
    {
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double BrentPriceHighLimit { get; set; } = 105;
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double BrentPriceHighSellPercent { get; set; } = 0.4;
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double BrentPriceLowLimit { get; set; } = 70;
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double BrentPriceLowBuyPercent { get; set; } = 0.4;
        [JsonConverter(typeof(StringToDecimalConverter))]
        public double MonthlyInvestment { get; set; } = 1000;
        [JsonConverter(typeof(StringToIntConverter))]
        public int DayOfTheMonthToInvest { get; set; } = 1;
    }

    public BrentHighLowSettings Settings { get; set; } = new BrentHighLowSettings();

    public override string Name => "Brent High Low Strategy";
    public override string Description => $"This strategy invests based on the price of BZ=F. If the price is lower than {Settings.BrentPriceLowLimit}, it buys the stock with {Settings.BrentPriceLowBuyPercent * 100}% of the InAccount balance. If the price is higher than {Settings.BrentPriceHighLimit}, it sells {Settings.BrentPriceHighSellPercent * 100}% of the monthly investment amount.";
    public override string[] UsingSymbols => ["BZ=F"];

    public BrentHighLowStrategy()
    {
    }

    public override StrategyResults RunBackTest(string strategyName, string symbol, Dictionary<string, Chart> charts, WalletBalance wallet)
    {
        var mainChart = charts[symbol];
        var bzChart = charts["BZ=F"];
        return ExecuteStrategy(strategyName, mainChart, wallet, (candle, wallet) =>
        {
            var bzCandle = bzChart.Candles.Values.FirstOrDefault(x => x.Date == candle.Date);

            if (bzCandle != null)
            {
                if (bzCandle.Open < Settings.BrentPriceLowLimit)
                {
                    var opportunityInfo = $"The price of Brent has dropped below $ {Settings.BrentPriceLowLimit}. " +
                    $"In this scenario, it is recommended to buy {Settings.BrentPriceLowBuyPercent}% of wallet in {symbol}.";
                    return ProcessBuy(candle, wallet, Settings.BrentPriceLowBuyPercent, opportunityInfo);
                }
                else if (bzCandle.Open >= Settings.BrentPriceHighLimit)
                {
                    var opportunityInfo = $"The price of Brent has reached the limit of $ {Settings.BrentPriceHighLimit}. " +
                    $"In this scenario, it is recommended to sell {Settings.BrentPriceHighSellPercent}% of {symbol} in custody.";
                    return ProcessSell(candle, wallet, Settings.BrentPriceHighSellPercent, opportunityInfo);
                }
            }
            return null;
        }, Settings.DayOfTheMonthToInvest);
    }

    public override double MonthlyInvestment => Settings.MonthlyInvestment;
    public override string GetSettingsJson()
    {
        return Settings.ToJson();
    }
    public override bool LoadSettingsFromJson(string content)
    {
        Settings = JsonSerializer.Deserialize<BrentHighLowSettings>(content);
        return true;
    }
}