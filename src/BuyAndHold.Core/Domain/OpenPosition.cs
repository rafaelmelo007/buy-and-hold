namespace BuyAndHold.Core.Domain;
public record OpenPosition
{
    public record PositionPart
    {
        public int Quantity { get; set; }
        public double InitialPrice { get; init; }
        public double LatestPrice { get; set; }
        public bool IsEmpty => Quantity == 0;

        public void SetLatestPrice(double price)
        {
            LatestPrice = price;
        }

        public int Subtract(int quantity, out double amountToAccount)
        {
            amountToAccount = quantity * LatestPrice;

            if (quantity >= Quantity)
            {
                quantity -= Quantity;
                Quantity = 0;
                return quantity;
            }

            Quantity -= quantity;
            return 0;
        }
    }

    public string Symbol { get; init; }
    public List<PositionPart> Data { get; init; } = new();
    public int Quantity => Data.Sum(x => x.Quantity);
    public double AveragePrice => Data.Any() ? Data.Sum(x => x.InitialPrice * x.Quantity) / Quantity : double.NegativeZero;
    public double CurrentPrice => Data.Any() ? Data.Sum(x => x.LatestPrice * x.Quantity) / Quantity : double.NegativeZero;
    public bool IsEmpty => Quantity == 0;

    public double AppendTrade(Order order, double accountAmountNow)
    {
        double accountAmountChange = 0;

        // Update the latest price for all existing parts
        Data.ForEach(part => part.SetLatestPrice(order.UnitPrice));

        if (order.Type == OrderType.Buy)
        {
            // Calculate the cost for the buy order
            accountAmountChange = -(order.Quantity * order.UnitPrice);
            var resultingAmount = accountAmountNow + accountAmountChange;

            // Ensure sufficient funds before executing the trade
            if (resultingAmount < 0)
                throw new InvalidOperationException($"Insufficient funds to buy {order.Quantity} units of {order.Symbol}. Resulting balance: {resultingAmount}");

            // Add the new position part
            Data.Add(new PositionPart
            {
                Quantity = order.Quantity,
                InitialPrice = order.UnitPrice,
                LatestPrice = order.UnitPrice,
            });
        }
        else if (order.Type == OrderType.Sell)
        {
            // Check if the quantity to sell exceeds the owned quantity
            if (Quantity < order.Quantity)
                throw new InvalidOperationException($"Cannot sell {order.Quantity} units of {order.Symbol}. Only {Quantity} units available.");

            int remainingQuantity = order.Quantity;
            while (remainingQuantity > 0)
            {
                var part = Data.First();
                remainingQuantity = part.Subtract(remainingQuantity, out var toAccount);
                accountAmountChange += toAccount;

                if (part.IsEmpty)
                    Data.Remove(part);
            }
        }

        return accountAmountChange;
    }

}
