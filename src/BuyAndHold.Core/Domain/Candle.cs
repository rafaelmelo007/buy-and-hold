namespace BuyAndHold.Core.Domain;
public class Candle
{
    public string? Symbol { get; set; }
    public DateOnly Date { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double Low { get; set; }
    public double High { get; set; }
    public double? Average21 { get; set; }
    public double? Average50 { get; set; }
    public double? Average200 { get; set; }
    public double Volume { get; set; }
    public string Comments { get; set; }
    public CandleType CandleType => GetCandleType();

    private CandleType GetCandleType()
    {
        if (Close > Open && (High - Low) > 2 * (Close - Open) && (Close - Open) > (Open - Low) && (High - Close) > (Close - Open))
        {
            return CandleType.Hammer;
        }
        else if (Open > Close && (High - Low) > 2 * (Open - Close) && (Open - Close) > (Close - Low) && (High - Open) > (Open - Close))
        {
            return CandleType.InvertedHammer;
        }
        else if (Math.Abs(Open - Close) < 0.1D * (High - Low))
        {
            return CandleType.Doji;
        }

        return CandleType.Unknown;
    }
}