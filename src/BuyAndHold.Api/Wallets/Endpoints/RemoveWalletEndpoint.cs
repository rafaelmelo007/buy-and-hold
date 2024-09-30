namespace BuyAndHold.Api.Wallets.Endpoints;
public class RemoveWalletEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapDelete($"/remove-wallet", Handle)
        .Produces<RemoveWalletResponse>()
        .WithSummary("Delete wallet from user");

    // Request / Response
    public record RemoveWalletRequest(long WalletId);
    public record RemoveWalletResponse(long Data);

    // Handler
    public static async Task<Ok<RemoveWalletResponse>> Handle(
         [FromServices] IWalletsService walletsService,
         [AsParameters] RemoveWalletRequest request,
         CancellationToken cancellationToken)
    {
        var id = await walletsService.RemoveWalletAsync(
            request.WalletId, cancellationToken);

        return TypedResults.Ok(new RemoveWalletResponse(id));
    }
}
