namespace BuyAndHold.Api.MarketBreadth.Endpoints;
public class GetWalletBreadthEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/get-wallet-breadth", Handle)
           .Produces<GetWalletBreadthResponse>()
           .WithSummary("Retrieve market breadth of a wallet");

    // Request / Response Records
    public record GetWalletBreadthRequest(long WalletId);
    public record GetWalletBreadthResponse(IEnumerable<AverageLines> Data);

    // Handler
    public static async Task<IResult> Handle(
         [FromServices] IWalletsService walletsService,
         [FromServices] IMarketBreadthService marketBreadthService,
         [AsParameters] GetWalletBreadthRequest request,
         CancellationToken cancellationToken)
    {
        var wallet = await walletsService.GetWalletWithSymbolsAsync(request.WalletId, cancellationToken);
        var symbols = wallet.Symbols.Select(x => x.Symbol).ToList();
        if (!symbols.Any()) return TypedResults.BadRequest("No symbols found");

        var lines = await marketBreadthService.GetWalletBreadthAsync(request.WalletId, cancellationToken);

        return TypedResults.Ok(new GetWalletBreadthResponse(lines));
    }

}
