using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BuyAndHold.Api.DailyStockPrices.Endpoints;
public class ImportFromBrapiDevEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapGet($"/import-from-brapi-dev", Handle)
        .Produces<ImportFromBrapiDevResponse>()
        .AllowAnonymous() // temporary
        .WithSummary("Save daily stock prices from Brapi.Dev Api");

    // Request / Response
    public record ImportFromBrapiDevRequest(string Symbol);
    public record ImportFromBrapiDevResponse(int TotalImported);

    public record BrapiDevResponse
    {
        public StockData[] Results { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Took { get; set; }
    }
    public record StockData
    {
        public string Currency { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public double RegularMarketChange { get; set; }
        public double RegularMarketChangePercent { get; set; }
        public DateTime RegularMarketTime { get; set; }
        public double RegularMarketPrice { get; set; }
        public double RegularMarketDayHigh { get; set; }
        public string RegularMarketDayRange { get; set; }
        public double RegularMarketDayLow { get; set; }
        public long RegularMarketVolume { get; set; }
        public double RegularMarketPreviousClose { get; set; }
        public double RegularMarketOpen { get; set; }
        public string FiftyTwoWeekRange { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public string Symbol { get; set; }
        public string UsedInterval { get; set; }
        public string UsedRange { get; set; }
        public List<HistoricalDataPrice>? HistoricalDataPrice { get; set; }
        public List<string> ValidRanges { get; set; }
        public List<string> ValidIntervals { get; set; }
        public double? PriceEarnings { get; set; }
        public double? EarningsPerShare { get; set; }
        public string LogoUrl { get; set; }
    }

    public record HistoricalDataPrice
    {
        public long Date { get; set; }
        public double? Open { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public long? Volume { get; set; }
        public double? AdjustedClose { get; set; }
    }

    // Handler
    public static async Task<IResult> Handle(
         [AsParameters] ImportFromBrapiDevRequest request,
         IOptions<VendorsConfiguration> vendorsConfiguration,
         IDateTimeService dateTimeService, IImportUrlService importUrlService,
         BuyAndHoldDbContext database,
         CancellationToken cancellationToken)
    {
        if (vendorsConfiguration?.Value is null) return TypedResults.BadRequest();

        var vendorUrl = vendorsConfiguration.Value.BrapiDevBaseUrl;
        var apiKey = vendorsConfiguration.Value.BrapiDevApiKey;

        var symbols = request.Symbol.Split(',').Select(x => x.Trim());

        var totalImported = 0;
        foreach (var symbol in symbols)
        {
            var url = $"{vendorUrl}/quote/{symbol}?token={apiKey}&interval=1d&range=5y";

            var content = await importUrlService.GetHttpRequestAsync(url, symbol);

            var response = JsonSerializer.Deserialize<BrapiDevResponse>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            var result = response.Results?.FirstOrDefault();
            if (result is null) return TypedResults.BadRequest();

            foreach (var row in result.HistoricalDataPrice)
            {
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(row.Date).DateTime;

                if (date.Date == dateTimeService.NowUtc.Date)
                {
                    continue;
                }

                var basePrice = row.Close ?? row.Open ?? row.Low ?? row.High ?? -1;
                if (basePrice <= 0)
                {
                    continue;
                }

                var adjustFactor = basePrice > 0 ? ((row.AdjustedClose / row.Close ?? basePrice)) : 1;

                var item = new DailyStockPrice
                {
                    Symbol = symbol,
                    Date = DateOnly.FromDateTime(date),
                    Open = (row.Open ?? basePrice) * adjustFactor,
                    Close = (row.Close ?? basePrice) * adjustFactor,
                    High = (row.High ?? basePrice) * adjustFactor,
                    Low = (row.Low ?? basePrice) * adjustFactor,
                    Volume = row.Volume.GetValueOrDefault(-1),
                    CreatedAt = DateTime.UtcNow
                };

                var first = database.DailyStockPrices.FirstOrDefault(x => x.Symbol == item.Symbol && x.Date == item.Date);
                if (first is null)
                {
                    database.DailyStockPrices.Add(item);
                    database.SaveChanges();
                    totalImported++;
                }
            }
        }


        return TypedResults.Ok(new ImportFromBrapiDevResponse(totalImported));
    }
}
