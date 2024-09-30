namespace BuyAndHold.Api.Wallets.Domain;
public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(f => f.WalletId);

        builder.Property(f => f.WalletId)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Name)
            .HasMaxLength(100);
    }
}