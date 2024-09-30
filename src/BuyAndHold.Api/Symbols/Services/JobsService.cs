namespace BuyAndHold.Api.Symbols.Services;
public class JobsService : IJobsService
{
    private readonly BuyAndHoldDbContext _database;
    private readonly ISymbolService _symbolService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IChartService _chartService;

    public JobsService(
        BuyAndHoldDbContext database,
        ISymbolService symbolService,
        IDateTimeService dateTimeService,
        IChartService chartService)
    {
        _database = database;
        _symbolService = symbolService;
        _dateTimeService = dateTimeService;
        _chartService = chartService;
    }

    public async Task<int> CalculateAveragesAsync(CancellationToken cancellationToken)
    {
        var affected = 0;
        var symbols = await _symbolService.GetSymbolsAsync(cancellationToken);
        var today = _dateTimeService.NowUtc.Date;
        foreach (var symbol in symbols)
        {
            var chart = await _chartService.GetChartAsync(symbol.Name, today.AddYears(-10).ToDateOnly(), today.ToDateOnly(), cancellationToken);
            foreach (var candle in chart.Candles)
            {
                var row = _database.DailyStockPrices.FirstOrDefault(x => x.Symbol == symbol.Name && x.Date == candle.Key);
                if (row is null) continue;

                row.Average21 = candle.Value.Average21;
                row.Average50 = candle.Value.Average50;
                row.Average200 = candle.Value.Average200;
            }
            affected += await _database.SaveChangesAsync();
        }
        return affected;
    }

    public async Task<int> InspectOpportunitiesAsync(CancellationToken cancellationToken)
    {
        var affected = 0;
        var symbols = await _symbolService.GetSymbolsAsync(cancellationToken);
        var today = _dateTimeService.NowUtc.Date.ToDateOnly();
        foreach (var symbol in symbols)
        {
            var begin = _dateTimeService.NowUtc.Date.AddYears(-2);
            var chart = await _chartService.GetChartAsync(symbol.Name, begin.ToDateOnly(), today, cancellationToken);
            symbol.LastPriceDate = chart.LastPriceDate;
            symbol.Candle100DHigh = chart.GetHighFromPeriod(today.AddWorkingDays(-100), today);
            symbol.Candle100DLow = chart.GetLowFromPeriod(today.AddWorkingDays(-100), today);

            symbol.Candle200DHigh = chart.GetHighFromPeriod(today.AddWorkingDays(-200), today.AddWorkingDays(-100));
            symbol.Candle200DLow = chart.GetLowFromPeriod(today.AddWorkingDays(-200), today.AddWorkingDays(-100));

            symbol.Candle300DHigh = chart.GetHighFromPeriod(today.AddWorkingDays(-300), today.AddWorkingDays(-200));
            symbol.Candle300DLow = chart.GetLowFromPeriod(today.AddWorkingDays(-300), today.AddWorkingDays(-200));

            var highest = new[] { symbol.Candle100DHigh, symbol.Candle200DHigh, symbol.Candle300DHigh }.Max();
            var lowest = new[] { symbol.Candle100DLow, symbol.Candle200DLow, symbol.Candle300DLow }.Min();

            symbol.LastPrice = chart.LastPrice;

            // Calculate the range
            var range = highest - lowest;

            // Avoid division by zero in case of an invalid range
            if (range.HasValue && range > 0 && symbol.LastPrice.HasValue)
            {
                symbol.PercentToLow = ((symbol.LastPrice.Value - lowest) / range.Value) * 100;
                symbol.PercentToHigh = 100 - symbol.PercentToLow;
            }
            else
            {
                symbol.PercentToHigh = null;
                symbol.PercentToLow = null;
            }

            affected += await _database.SaveChangesAsync();
        }
        return affected;
    }

}
