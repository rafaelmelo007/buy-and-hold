namespace BuyAndHold.Api.DailyStockPrices.Domain;
public class DailyStockPriceConfiguration : IEntityTypeConfiguration<DailyStockPrice>
{
    public void Configure(EntityTypeBuilder<DailyStockPrice> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Symbol)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(f => f.Date)
            .IsRequired();

        builder.Property(f => f.Open)
            .IsRequired();

        builder.Property(f => f.High)
            .IsRequired();

        builder.Property(f => f.Low)
            .IsRequired();

        builder.Property(f => f.Close)
            .IsRequired();
    }
}