using BuyAndHold.Api.MarketMood.Endpoints;

namespace BuyAndHold.Api.MarketMood;
public static class Setup
{
    public static void MapMarketMoodEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/market-mood")
            .WithTags("MarketMood")
            .RequireAuthorization()
            .MapEndpoint<GetMarketMoodEndpoint>();
    }

}
