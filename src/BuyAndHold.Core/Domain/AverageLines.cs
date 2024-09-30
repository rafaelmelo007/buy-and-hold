namespace BuyAndHold.Core.Domain;
public record AverageLines
{
    public DateOnly Date { get; set; }
    public double? Average21 { get; set; }
    public double? Average50 { get; set; }
    public double? Average200 { get; set; }
    public string Comments { get; set; }
}
