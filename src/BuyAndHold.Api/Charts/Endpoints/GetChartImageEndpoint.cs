namespace BuyAndHold.Api.Charts.Endpoints;

public class GetChartImageEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/chart-image", Handle)
        .Produces<FileContentResult>(200, "application/octet-stream")
        .WithSummary("Generate and download a chart image of a symbol to a given period");

    // Request / Response
    public record GetCandlesImageRequest(string Symbol, DateOnly Begin, DateOnly End);

    // Handler
    public static async Task<IResult> Handle(
         [FromServices] IChartService chartService,
         [FromServices] IChartImageBuilder chartBuilder,
         [AsParameters] GetCandlesImageRequest request,
         CancellationToken cancellationToken)
    {
        Chart chart = await chartService.GetChartAsync(request.Symbol, request.Begin, request.End, cancellationToken);
        var res = chartBuilder.Build(chart);

        // Return the image as a downloadable file
        var response = Results.File(res.FileContent, res.MimeType, res.FileName);

        return response;
    }

}
