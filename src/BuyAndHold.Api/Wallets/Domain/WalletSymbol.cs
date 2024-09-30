using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuyAndHold.Api.Wallets.Domain;
public record WalletSymbol
{
    public long WalletSymbolId { get; set; }
    public string? Symbol { get; set; }
    public int Quantity { get; set; }
    public double? AveragePrice { get; set; }
    public int ExpectedQuantity { get; set; }
    public long? WalletId { get; set; }
    [JsonIgnore]
    public Wallet? Wallet { get; set; }
}
