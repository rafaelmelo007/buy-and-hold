namespace BuyAndHold.Api.Wallets.Domain;
public class WalletSymbolConfiguration : IEntityTypeConfiguration<WalletSymbol>
{
    public void Configure(EntityTypeBuilder<WalletSymbol> builder)
    {
        builder.HasKey(f => f.WalletSymbolId);

        builder.Property(f => f.WalletSymbolId)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Symbol)
            .HasMaxLength(100);
    }
}