namespace BuyAndHold.Api.Symbols.Endpoints;
public class GetSymbolsEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/get-symbols", Handle)
        .Produces<GetSymbolsResponse>()
        .WithSummary("Retrieve all symbols handled by the system");

    // Request / Response
    public record GetSymbolsRequest();
    public record GetSymbolsResponse(IEnumerable<Symbol> Data);

    public record SymbolDto(long Id, string Symbol, DateTime LastUpdatedAt,
        decimal LastPrice, DateOnly LastPriceDate,
        decimal Last30DaysVariation, decimal Last100DaysVariation,
        decimal Last6MonthsVariation);

    // Handler
    public static async Task<Ok<GetSymbolsResponse>> Handle(
         [FromServices] ISymbolService symbolService,
         [AsParameters] GetSymbolsRequest request,
         CancellationToken cancellationToken)
    {
        var symbols = await symbolService.GetSymbolsAsync(cancellationToken);

        return TypedResults.Ok(new GetSymbolsResponse(symbols));
    }
}
