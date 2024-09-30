namespace BuyAndHold.Core.Domain;
public record StrategyResultCandle
{
    public StrategyResultCandle() { }

    public StrategyResultCandle(string symbol, WalletBalance balance, Candle candle, Order? orderExecuted, FundInjection? fundAdded)
    {
        Date = candle.Date;
        Open = candle.Open;
        Close = candle.Close;
        High = candle.High;
        Low = candle.Low;
        balance.SetCurrentPrice(symbol, candle.Close);
        AmountInWallet = balance.InWallet;
        if (balance.Positions.ContainsKey(symbol))
        {
            AveragePrice = balance.Positions[symbol].AveragePrice;
            AmountInCustody = balance.Positions[symbol].Quantity * candle.Close;
            QuantityInCustody = balance.Positions[symbol].Quantity;
            var totalInvested = (balance.Positions[symbol].Quantity * balance.Positions[symbol].AveragePrice) + balance.InWallet;
            var totalReceived = (balance.Positions[symbol].Quantity * balance.Positions[symbol].CurrentPrice) + balance.InWallet;
            ProfitPercentage = ((totalReceived / totalInvested) - 1) * 100;
        }
        OrderExecuted = orderExecuted;
        FundAdded = fundAdded;
    }

    public int Year => Date.Year;
    public DateOnly Date { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double AmountInWallet { get; set; }
    public double AveragePrice { get; set; }
    public double AmountInCustody { get; set; }
    public int QuantityInCustody { get; set; }
    public double ProfitPercentage { get; set; }
    public Order? OrderExecuted { get; set; }
    public FundInjection? FundAdded { get; set; }
    public string OrderDescription => OrderExecuted?.ToDescription();

}

