namespace BuyAndHold.Api.DailyStockPrices.Domain;
public record DailyStockPrice
{
    public long Id { get; set; }
    public string? Symbol { get; set; }
    public DateOnly Date { get; set; }
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public double? Volume { get; set; }
    public double? Average21 { get; set; }
    public double? Average50 { get; set; }
    public double? Average200 { get; set; }
    public DateTime CreatedAt { get; set; }
}
