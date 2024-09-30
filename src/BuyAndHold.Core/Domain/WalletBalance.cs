namespace BuyAndHold.Core.Domain;
public record WalletBalance
{
    public double InitialInWallet { get; init; }
    public DateOnly InitialDate { get; init; }
    public Dictionary<string, OpenPosition> Positions { get; init; } = new();
    public double InWallet { get; set; }
    public double InCustody => Positions.Sum(x => x.Value.Quantity * x.Value.CurrentPrice);
    public List<Order> Orders { get; set; } = new List<Order>();
    public List<FundInjection> FundInjections { get; set; } = new List<FundInjection>();
    public Dictionary<int, double> ResultYears { get; set; } = new Dictionary<int, double>();

    public void StoreYearMetrics(int year)
    {
        var totalInvested = InitialInWallet + FundInjections.Sum(x => x.Amount);
        var totalReceived = InWallet + InCustody;
        var totalProfit = CalculateProfit();
        var profitVariation = totalInvested > 0 ? (totalReceived / totalInvested - 1) * 100 : default;
        ResultYears[year] = profitVariation;
    }

    public void AppendOrder(Order order)
    {
        order.InWalletBefore = InWallet;

        if (string.IsNullOrEmpty(order.Symbol))
            throw new ArgumentNullException(nameof(order.Symbol));

        Orders.Add(order);

        if (InWallet == double.NegativeZero && !Positions.Any())
            InWallet = InitialInWallet;

        if (!Positions.TryGetValue(order.Symbol, out var position))
        {
            position = new OpenPosition { Symbol = order.Symbol };
            Positions[order.Symbol] = position;
        }

        InWallet += position.AppendTrade(order, InWallet);

        order.InWalletAfter = InWallet;

        if (position.IsEmpty)
            Positions.Remove(order.Symbol);
    }

    public void SetCurrentPrice(string symbol, double price)
    {
        if (Positions.TryGetValue(symbol, out var position))
        {
            position.Data.ForEach(part => part.SetLatestPrice(price));
        }
    }

    public void AddFunds(DateOnly date, double amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount to add must be positive.");

        FundInjections.Add(new FundInjection { Date = date, Amount = amount });

        InWallet += amount;
    }

    public double CalculateProfit()
    {
        return InWallet + InCustody - InitialInWallet;
    }
}
