
namespace BuyAndHold.Api.MarketBreadth.Services;
public class MarketBreadthService : IMarketBreadthService
{
    private readonly IChartService _chartService;
    private readonly IDateTimeService _dateTimeService;

    public MarketBreadthService(IChartService chartService,
        IDateTimeService dateTimeService)
    {
        _chartService = chartService;
        _dateTimeService = dateTimeService;
    }

    public async Task<IEnumerable<AverageLines>> GetWalletBreadthAsync(long walletId, CancellationToken cancellationToken)
    {
        var end = _dateTimeService.NowUtc.Date.ToDateOnly();
        var begin = end.AddWorkingDays(-600);
        var chart = await _chartService.GetWalletChartAsync(walletId, begin, end, cancellationToken);

        var lines = chart.Candles.Values.Select(x =>
        {
            var averageLines = new AverageLines
            {
                Date = x.Date,
                Average21 = x.Average21,
                Average50 = x.Average50,
                Average200 = x.Average200,
                Comments = x.Comments
            };
            return averageLines;
        }).OrderByDescending(x => x.Date).ToList();

        return lines;
    }
}
