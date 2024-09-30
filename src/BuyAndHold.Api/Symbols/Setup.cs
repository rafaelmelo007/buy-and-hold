using BuyAndHold.Api.Symbols.Endpoints;

namespace BuyAndHold.Api.Symbols;
public static class Setup
{
    public static void MapSymbolsEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/symbols")
            .WithTags("Symbols")
            .RequireAuthorization()
            .MapEndpoint<GetSymbolsEndpoint>()
            .MapEndpoint<RunJobsEndpoint>();
    }

}
