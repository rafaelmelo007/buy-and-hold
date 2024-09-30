namespace BuyAndHold.Api.Wallets.Domain;
public record Wallet
{
    public long WalletId { get; set; }
    public string? Name { get; set; }
    public long UserId { get; set; }
    public bool IsDefault { get; set; }
    public ICollection<WalletSymbol>? Symbols { get; set; }
}
