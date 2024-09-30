namespace BuyAndHold.Api.DailyStockPrices.Domain;
public class ImportedUrlConfiguration : IEntityTypeConfiguration<ImportedUrl>
{
    public void Configure(EntityTypeBuilder<ImportedUrl> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Url)
            .HasMaxLength(500);
    }
}