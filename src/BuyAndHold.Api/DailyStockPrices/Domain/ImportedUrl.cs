namespace BuyAndHold.Api.DailyStockPrices.Domain;
public record ImportedUrl
{
    public long Id { get; set; }
    public string? Url { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
