namespace BuyAndHold.Core.Domain;
public record Order
{
    public string? Symbol { get; init; }
    public double UnitPrice { get; init; }
    public int Quantity { get; init; }
    public DateOnly Date { get; init; }
    public OrderType Type { get; init; }
    public IInvestStrategy? Strategy { get; init; }
    public double InWalletBefore { get; set; }
    public double InWalletAfter { get; set; }
    public string? OpportunityInfo { get; set; }

    public string ToDescription()
    {
        var text = $"{(Type == OrderType.Buy ? "Buy" : "Sell")} {Quantity} units of {Symbol} with the price {UnitPrice}";
        return text;
    }
}