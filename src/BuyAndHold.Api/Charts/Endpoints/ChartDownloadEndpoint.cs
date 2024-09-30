namespace BuyAndHold.Api.Charts.Endpoints;
public class ChartDownloadEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/chart-download", Handle)
        .Produces<FileContentResult>(200, "application/octet-stream")
        .WithSummary("Generate a chart file including all the candles of a symbol to a given period");

    // Request / Response
    public record ChartDownloadRequest(string Symbol, DateOnly? Begin, DateOnly? End);

    // Handler
    public static async Task<IResult> Handle(
         [FromServices] IChartService chartService,
         [FromServices] IChartImageBuilder chartBuilder,
         [AsParameters] ChartDownloadRequest request,
         CancellationToken cancellationToken)
    {
        Chart chart = await chartService.GetChartAsync(request.Symbol, request.Begin ?? DateOnly.MinValue, request.End ?? DateOnly.MaxValue, cancellationToken);
        var res = chart.ToChartFile();

        // Return the image as a downloadable file
        var response = Results.File(res.FileContent, res.MimeType, res.FileName);

        return response;
    }

}
