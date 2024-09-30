namespace BuyAndHold.Api.Charts.Services;
public class ChartService : IChartService
{
    private readonly BuyAndHoldDbContext _database;
    private readonly ICurrentUserService _currentUserService;

    public enum AboveBelow
    {
        Above = 1,
        Below = 2,
        None = 3
    }

    public ChartService(BuyAndHoldDbContext database,
        ICurrentUserService currentUserService)
    {
        _database = database;
        _currentUserService = currentUserService;
    }

    public async Task<Chart> GetChartAsync(string symbol, DateOnly begin, DateOnly end, CancellationToken cancellationToken)
    {
        var rows = await _database.DailyStockPrices.Where(x => x.Symbol == symbol && x.Date >= begin && x.Date <= end).OrderBy(x => x.Date).ToListAsync(cancellationToken);

        var chart = new Chart
        {
            Symbol = symbol,
            Begin = begin,
            End = end,
            Candles = rows.ToCandles()
        };

        return chart;
    }

    public async Task<Chart> GetWalletChartAsync(long wallertId, DateOnly begin, DateOnly end, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var symbols = await _database.WalletSymbols.Where(x => x.Wallet.UserId == userId && x.WalletId == wallertId).Select(x => x.Symbol).ToListAsync(cancellationToken);

        var charts = symbols.Select(symbol =>
        {
            var chart = GetChartAsync(symbol, begin, end, cancellationToken).GetAwaiter().GetResult();
            return new { symbol, chart };
        }).ToDictionary(x => x.symbol, x => x.chart);

        var unifiedChart = GroupChart(charts, $"Wallet #{wallertId}");
        return unifiedChart;
    }

    #region Helper Methods

    private Chart GroupChart(IDictionary<string, Chart> charts, string name)
    {
        var begin = charts.Values.Select(x => x.Begin).Min();
        var end = charts.Values.Select(x => x.End).Max();
        var chart = new Chart { Symbol = name, Begin = begin, End = end, Candles = new Dictionary<DateOnly, Candle>() };
        foreach (var childChart in charts.Values)
        {
            foreach (var childCandle in childChart.Candles.Values)
            {
                var candle = chart.Candles.Values.FirstOrDefault(x => x.Date == childCandle.Date);
                if (candle is null)
                {
                    candle = new Candle
                    {
                        Date = childCandle.Date,
                        Open = 0,
                        High = 0,
                        Close = 0,
                        Low = 0,
                        Volume = 0,
                        Average21 = 0,
                        Average50 = 0,
                        Average200 = 0,
                        Comments = string.Empty
                    };
                    chart.Candles.Add(candle.Date, candle);
                }

                candle.Open += childCandle.Open;
                candle.High += childCandle.High;
                candle.Close += childCandle.Close;
                candle.Low += childCandle.Low;
                candle.Volume += childCandle.Volume;

                var result = CalculateAverage(childCandle.Average21, candle.Close);
                if (result == AboveBelow.Above)
                {
                    candle.Average21 += 1;
                    candle.Comments += $"[above 21 - {childCandle.Symbol}]";
                }
                else if (result == AboveBelow.Below)
                {
                    candle.Average21 -= 1;
                    candle.Comments += $"[below 21 - {childCandle.Symbol}]";
                }

                result = CalculateAverage(childCandle.Average50, candle.Close);
                if (result == AboveBelow.Above)
                {
                    candle.Average50 += 1;
                    candle.Comments += $"[above 50 - {childCandle.Symbol}]";
                }
                else if (result == AboveBelow.Below)
                {
                    candle.Average50 -= 1;
                    candle.Comments += $"[below 50 - {childCandle.Symbol}]";
                }

                result = CalculateAverage(childCandle.Average200, candle.Close);
                if (result == AboveBelow.Above)
                {
                    candle.Average200 += 1;
                    candle.Comments += $"[above 200 - {childCandle.Symbol}]";
                }
                else if (result == AboveBelow.Below)
                {
                    candle.Average200 -= 1;
                    candle.Comments += $"[below 200 - {childCandle.Symbol}]";
                }
            }
        }

        return chart;
    }

    private AboveBelow CalculateAverage(double? average, double close)
    {
        if (!average.HasValue) return AboveBelow.None;
        if (average.Value > close) return AboveBelow.Below;
        if (average.Value < close) return AboveBelow.Above;
        return 0;
    }

    #endregion
}
