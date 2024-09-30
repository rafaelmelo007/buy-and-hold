using BuyAndHold.Api.DailyStockPrices.Endpoints;

namespace BuyAndHold.Api.Authentication;
public static class DailyStockPrices
{
    public static void MapDailyStockPricesEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/daily-stock-prices")
            .WithTags("DailyStockPrices")
            .RequireAuthorization()
            .MapEndpoint<ImportFromBrapiDevEndpoint>();
    }

}
