namespace BuyAndHold.Api.Wallets.Endpoints;
public class GetWalletsEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/get-wallets", Handle)
        .Produces<GetWalletResponse>()
        .WithSummary("Retrieve wallets from user");

    // Request / Response
    public record GetWalletRequest();
    public record GetWalletResponse(IEnumerable<WalletDto>? Wallets);

    public record WalletDto
    {
        public long WalletId { get; set; }
        public string? Name { get; set; }
        public long UserId { get; set; }
        public bool IsDefault { get; set; }
        public List<WalletSymbolDto>? Symbols { get; set; }
        public double TotalAmount { get; set; }
        public double ExpectedTotalAmount { get; set; }
        public double PercentCompleted { get; set; }
    }

    public record WalletSymbolDto
    {
        public long WalletSymbolId { get; set; }
        public string? Symbol { get; set; }
        public int Quantity { get; set; }
        public double? AveragePrice { get; set; }
        public int ExpectedQuantity { get; set; }
        public long? WalletId { get; set; }
        public double TotalAmount { get; set; }
        public double ExpectedTotalAmount { get; set; }
        public double PercentCompleted { get; set; }
    }

    // Handler
    public static async Task<Ok<GetWalletResponse>> Handle(
         [FromServices] IWalletsService walletsService,
         [FromServices] ISymbolService symbolService,
         [AsParameters] GetWalletRequest request,
         CancellationToken cancellationToken)
    {
        var wallets = await walletsService.GetWalletsWithSymbolsAsync();

        var prices = await symbolService.GetSymbolPricesAsync(cancellationToken);

        var mapped = wallets.Select(x => new WalletDto
        {
            WalletId = x.WalletId,
            Name = x.Name,
            UserId = x.UserId,
            IsDefault = x.IsDefault,
            TotalAmount = x.Symbols.Sum(x2 => x2.Quantity * prices[x2.Symbol]),
            ExpectedTotalAmount = x.Symbols.Sum(x2 => x2.ExpectedQuantity * prices[x2.Symbol]),
            Symbols = x.Symbols.Select(x2 => new WalletSymbolDto
            {
                WalletId = x2.WalletId,
                WalletSymbolId = x2.WalletSymbolId,
                Symbol = x2.Symbol,
                AveragePrice = x2.AveragePrice,
                Quantity = x2.Quantity,
                ExpectedQuantity = x2.ExpectedQuantity,
                TotalAmount = x2.Quantity * prices[x2.Symbol],
                ExpectedTotalAmount = x2.ExpectedQuantity * prices[x2.Symbol]
            }).ToList()
        }).ToList();

        mapped.ForEach(wallet =>
        {
            wallet.PercentCompleted = (wallet.TotalAmount / wallet.ExpectedTotalAmount) * 100;
            wallet.Symbols.ForEach(symbol =>
            {
                symbol.PercentCompleted = (symbol.TotalAmount / symbol.ExpectedTotalAmount) * 100;
            });
        });

        return TypedResults.Ok(new GetWalletResponse(mapped));
    }
}
