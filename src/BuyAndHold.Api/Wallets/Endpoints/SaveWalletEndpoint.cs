namespace BuyAndHold.Api.Wallets.Endpoints;
public class SaveWalletEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapPost($"/save-wallet", Handle)
        .Produces<SaveWalletResponse>()
        .WithSummary("Add or update particular wallet of user");

    // Request / Response
    public record SaveWalletRequest(Wallet Wallet);
    public record SaveWalletResponse(long Data);

    // Handler
    public static async Task<Ok<SaveWalletResponse>> Handle(
         [FromServices] IWalletsService walletsService,
         [FromBody] SaveWalletRequest request,
         CancellationToken cancellationToken)
    {
        var id = await walletsService.AddOrUpdateWalletAsync(
            request.Wallet, cancellationToken);

        return TypedResults.Ok(new SaveWalletResponse(id));
    }
}
