namespace BuyAndHold.Api.Data;
public static class AddPersistenceExtension
{
    public static IServiceCollection AddPersistence(
    this IServiceCollection services, IConfiguration configuration,
    bool unitTestMode = false)
    {
        var dataConfiguration = configuration.GetSection(nameof(DataConfiguration)).Get<DataConfiguration>();
        services.Configure<DataConfiguration>(configuration.GetSection(nameof(DataConfiguration)));

        if (!string.IsNullOrEmpty(dataConfiguration.ConnectionString))
        {
            services.AddDbContext<BuyAndHoldDbContext>(options =>
                options
                .UseSqlServer(dataConfiguration.ConnectionString, x => x.UseCompatibilityLevel(120))
                .EnableSensitiveDataLogging());

            // Run DB Migrations
            var provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<BuyAndHoldDbContext>();
            if (dataConfiguration.RunMigrations)
            {
                context.Database.Migrate();
            }
        }
        else
        {
            var databaseName = unitTestMode ?
                $"BuyAndHold.Api.EmptyDatabase-{Guid.NewGuid()}" :
                $"BuyAndHold.Api.EmptyDatabase";
            services.AddDbContext<BuyAndHoldDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        }
        return services;
    }

}
