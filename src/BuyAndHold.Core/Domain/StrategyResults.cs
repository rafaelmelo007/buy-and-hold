namespace BuyAndHold.Core.Domain;
public class StrategyResults
{
    public string? SymbolName { get; set; }
    public string? StrategyName { get; set; }
    public double TotalInvested { get; set; }
    public double TotalReceived { get; set; }
    public double TotalInAccount { get; set; }
    public double TotalInCustody { get; set; }
    public double AveragePrice { get; set; }
    public double CurrentPrice { get; set; }
    public int TotalDaysHolding { get; set; }
    public int TotalMonthsHolding { get; set; }
    public int TotalYearsHolding { get; set; }
    public double TotalProfit { get; set; }
    public double ProfitVariation { get; set; }
    public int TotalTrades { get; set; }
    public int TotalQuantityInCustody { get; set; }
    public IEnumerable<StrategyResultCandle> Candles { get; set; }
    public Dictionary<int, double> ResultYears { get; set; }
    public DateOnly BeginDate { get; internal set; }
    public DateOnly EndDate { get; internal set; }

    public override string ToString()
    {
        return $"Total Invested: {TotalInvested:C}\n" +
               $"Total Received: {TotalReceived:C} (Variation: {ProfitVariation:P2})\n" +
               $"Average Price: {AveragePrice:C}\n" +
               $"Current Price: {CurrentPrice:C}\n" +
               $"Total In Account: {TotalInAccount:C}\n" +
               $"Total In Custody: {TotalInCustody:C}\n" +
               $"Total Days Holding: {TotalDaysHolding}\n" +
               $"Total Months Holding: {TotalMonthsHolding}\n" +
               $"Total Years Holding: {TotalYearsHolding}\n" +
               $"Total Trades: {TotalTrades}";
    }
}