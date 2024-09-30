using BuyAndHold.Api.Wallets.Endpoints;

namespace BuyAndHold.Api.Wallets;
public static class WalletsEndpoints
{
    public static void MapWalletsEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/wallets")
            .WithTags("Wallets")
            .RequireAuthorization()
            .MapEndpoint<GetWalletsEndpoint>()
            .MapEndpoint<SaveWalletEndpoint>()
            .MapEndpoint<RemoveWalletEndpoint>();
    }

}
