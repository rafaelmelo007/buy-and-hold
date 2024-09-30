using System.ComponentModel.DataAnnotations.Schema;

namespace BuyAndHold.Api.Symbols.Domain;
public record Symbol
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Aliases { get; set; }
    public long? ImportedUrlId { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public double? LastPrice { get; set; }
    public DateOnly? LastPriceDate { get; set; }
    public double? PercentToHigh { get; set; }
    public double? PercentToLow { get; set; }
    public double? Candle100DHigh { get; set; }
    public double? Candle100DLow { get; set; }
    public double? Candle200DHigh { get; set; }
    public double? Candle200DLow { get; set; }
    public double? Candle300DHigh { get; set; }
    public double? Candle300DLow { get; set; }
    [NotMapped]
    public long? BuyMood { get; set; }
    [NotMapped]
    public long? SellMood { get; set; }
}
