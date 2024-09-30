using BuyAndHold.Api.MarketBreadth.Endpoints;

namespace BuyAndHold.Api.MarketBreadth;
public static class Setup
{
    public static void MapMarketBreadthEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/market-breadth")
            .WithTags("MarketBreadth")
            .RequireAuthorization()
            .MapEndpoint<GetWalletBreadthEndpoint>();
    }

}
