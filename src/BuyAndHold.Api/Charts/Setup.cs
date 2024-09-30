using BuyAndHold.Api.Charts.Endpoints;

namespace BuyAndHold.Api.Charts;
public static class Setup
{
    public static void MapChartsEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/charts")
            .WithTags("Charts")
            .RequireAuthorization()
            .MapEndpoint<GetChartDataEndpoint>()
            .MapEndpoint<GetChartImageEndpoint>()
            .MapEndpoint<ChartDownloadEndpoint>();
    }

}
