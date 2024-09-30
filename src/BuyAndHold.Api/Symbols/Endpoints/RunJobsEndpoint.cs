namespace BuyAndHold.Api.Symbols.Endpoints;
public class RunJobsEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapPut($"/run-jobs", Handle)
        .Produces<CalculateSymbolsResponse>()
        .AllowAnonymous() // temporary
        .WithSummary("Run all the jobs to support the application");

    // Request / Response
    public record CalculateSymbolsRequest(bool InspectOpportunities = true, bool CalculateAverages = true);
    public record CalculateSymbolsResponse(int Affected);

    // Handler
    public static async Task<Ok<CalculateSymbolsResponse>> Handle(
         [FromServices] IJobsService opportunityService,
         [AsParameters] CalculateSymbolsRequest request,
         CancellationToken cancellationToken)
    {
        int affected = request.InspectOpportunities ? await opportunityService.InspectOpportunitiesAsync(cancellationToken) : 0;
        affected += request.CalculateAverages ? await opportunityService.CalculateAveragesAsync(cancellationToken) : 0;

        return TypedResults.Ok(new CalculateSymbolsResponse(affected));
    }
}
