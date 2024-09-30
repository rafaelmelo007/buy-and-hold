namespace BuyAndHold.Api.MarketMood.Endpoints;

public class GetMarketMoodEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/get-market-mood", Handle)
           .Produces<GetMarketMoodResponse>()
           .WithSummary("Retrieve market mood indicators");

    // Request / Response Records
    public record GetMarketMoodRequest();
    public record GetMarketMoodResponse(long BuyMood, long SellMood, IEnumerable<OpportunityDto> Opportunities);
    public record OpportunityDto(string Name, double? LastPrice, DateOnly? LastPriceDate, string ActionRequired, string OpportunityDescription, long Score);

    // Handler
    public static async Task<IResult> Handle(
         [FromServices] ISymbolService symbolService,
         [AsParameters] GetMarketMoodRequest request,
         CancellationToken cancellationToken)
    {
        var symbols = await symbolService.GetSymbolsAsync(cancellationToken);
        var filteredSymbols = FilterSymbolsByMood(symbols);

        var opportunities = filteredSymbols.Select(CreateOpportunityDto).ToList();
        var response = new GetMarketMoodResponse(
            BuyMood: filteredSymbols.Sum(x => x.BuyMood ?? 0),
            SellMood: filteredSymbols.Sum(x => x.SellMood ?? 0),
            Opportunities: opportunities);

        return TypedResults.Ok(response);
    }

    private static List<Symbol> FilterSymbolsByMood(IEnumerable<Symbol> symbols)
    {
        var result = symbols
            .Where(x => x.BuyMood >= 3 || x.SellMood >= 3)
            .ToList();
        return result;
    }

    private static OpportunityDto CreateOpportunityDto(Symbol symbol)
    {
        var actionRequired = DetermineAction(symbol);
        var opportunityDescription = GetOpportunityDescription(symbol);

        var result = new OpportunityDto(
            Name: symbol.Name,
            LastPrice: symbol.LastPrice,
            LastPriceDate: symbol.LastPriceDate,
            ActionRequired: actionRequired,
            OpportunityDescription: opportunityDescription,
            Score:
                symbol.BuyMood.HasValue && symbol.BuyMood.Value > 0
                    ? symbol.BuyMood.Value :
                    symbol.SellMood.HasValue && symbol.SellMood.Value > 0
                        ? symbol.SellMood.Value : 0);

        return result;
    }

    private static string DetermineAction(Symbol symbol)
    {
        var result = symbol.BuyMood > 0 ? "Buy" : "Sell";
        return result;
    }

    private static string GetOpportunityDescription(Symbol symbol)
    {
        var description = string.Empty;

        if (symbol.BuyMood > 0)
        {
            description = $"This symbol is being traded {symbol.PercentToLow:#.00}% close to the low of the last 300 days. In situations like this, we recommend buying it.";
        }
        else if (symbol.SellMood > 0)
        {
            description = $"This symbol is being traded {symbol.PercentToHigh:#.00}% close to the low of the last 300 days. In situations like this, we recommend selling 1% of it.";
        }

        return description;
    }
}
