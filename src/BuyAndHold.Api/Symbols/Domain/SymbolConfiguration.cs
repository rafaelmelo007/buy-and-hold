namespace BuyAndHold.Api.Symbols.Domain;
public class SymbolConfiguration : IEntityTypeConfiguration<Symbol>
{
    public void Configure(EntityTypeBuilder<Symbol> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.Aliases)
            .HasMaxLength(250);

        builder.Property(f => f.LastUpdatedAt)
            .IsRequired();
    }
}