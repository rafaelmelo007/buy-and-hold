namespace BuyAndHold.Api.Charts.Endpoints;

public class GetChartDataEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/chart-data", Handle)
        .Produces<GetCandlesDataResponse>()
        .WithSummary("Retrieve a chart of a symbol to a given period");

    // Request / Response
    public record GetCandlesDataRequest(string Symbol, DateOnly Begin, DateOnly End);
    public record GetCandlesDataResponse(Chart Data);

    // Handler
    public static async Task<Ok<GetCandlesDataResponse>> Handle(
         [FromServices] IChartService chartService,
         [AsParameters] GetCandlesDataRequest request,
         CancellationToken cancellationToken)
    {
        Chart chart = await chartService.GetChartAsync(request.Symbol, request.Begin, request.End, cancellationToken);
        return TypedResults.Ok(new GetCandlesDataResponse(chart));
    }
}
