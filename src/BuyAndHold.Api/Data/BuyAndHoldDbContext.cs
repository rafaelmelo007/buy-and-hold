namespace BuyAndHold.Api.Data;
public class BuyAndHoldDbContext : DbContext
{
    public readonly ICurrentUserService _currentUserService;
    public readonly IDateTimeService _dateTimeService;

    public BuyAndHoldDbContext(
        DbContextOptions<BuyAndHoldDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public static BuyAndHoldDbContext CreateEmptyDatabase(
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        var options = new DbContextOptionsBuilder<BuyAndHoldDbContext>();
        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var ctx = new BuyAndHoldDbContext(options.Options, currentUserService, dateTimeService);
        return ctx;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DailyStockPrice> DailyStockPrices { get; set; }
    public DbSet<ImportedUrl> ImportedUrls { get; set; }
    public DbSet<Symbol> Symbols { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletSymbol> WalletSymbols { get; set; }

    public override int SaveChanges()
    {
        var GetUserId = (long? userId) =>
        {
            var id = _currentUserService.UserId;
            return userId.HasValue && userId > 0 ? userId ?? -1 : id ?? -1;
        };

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = GetUserId(entry.Entity.CreatedBy);
                    entry.Entity.CreateDateUtc = _dateTimeService.NowUtc;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedBy = GetUserId(entry.Entity.UpdatedBy);
                    entry.Entity.UpdateDateUtc = _dateTimeService.NowUtc;
                    break;
                case EntityState.Deleted:
                    entry.Entity.DeletedBy = GetUserId(entry.Entity.DeletedBy);
                    entry.Entity.DeleteDateUtc = _dateTimeService.NowUtc;
                    entry.Entity.IsDeleted = true;
                    entry.State = EntityState.Modified;
                    break;
            }
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuyAndHoldDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

}
