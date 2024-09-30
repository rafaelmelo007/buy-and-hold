namespace BuyAndHold.Core.Renders;
public class ScottPlotChartImageBuilder : IChartImageBuilder
{
    public BinaryFile? Build(Chart chart, StrategyResults? strategyResults = default)
    {
        var data = chart.Candles.Select(x => new ScottPlot.OHLC
        {
            DateTime = x.Value.Date.ToDateTime(new TimeOnly()),
            Open = Convert.ToDouble(x.Value.Open),
            Close = Convert.ToDouble(x.Value.Close),
            High = Convert.ToDouble(x.Value.High),
            Low = Convert.ToDouble(x.Value.Low),
            TimeSpan = TimeSpan.FromHours(8)
        }).ToList();

        // Create a new Plot
        ScottPlot.Plot myPlot = new();

        // Add a candlestick series to the plot
        myPlot.Title($"{chart.Symbol} - [{chart.Begin.ToString("yyyy-MM-dd")} - {chart.End.ToString("yyyy-MM-dd")}]");
        myPlot.Add.Candlestick(data);
        myPlot.Axes.DateTimeTicksBottom();

        // Get the image bytes
        byte[] imageBytes = myPlot.GetImageBytes(5000, 500, ScottPlot.ImageFormat.Png);

        var result = new BinaryFile
        {
            MimeType = "image/svg",
            FileName = $"{chart.Symbol}_{chart.Begin:yyyyMMdd}_{chart.End:yyyyMMdd}.png",
            FileContent = imageBytes,
        };

        return result;
    }

    public BinaryFile? Build(string symbol, IEnumerable<StrategyResultCandle> candles)
    {
        var data = candles.Select(x => new ScottPlot.OHLC
        {
            DateTime = x.Date.ToDateTime(new TimeOnly()),
            Open = Convert.ToDouble(x.Open),
            Close = Convert.ToDouble(x.Close),
            High = Convert.ToDouble(x.High),
            Low = Convert.ToDouble(x.Low),
            TimeSpan = TimeSpan.FromHours(8)
        }).ToList();

        var beginDate = candles.Min(x => x.Date);
        var endDate = candles.Max(x => x.Date);

        // Create a new Plot
        ScottPlot.Plot myPlot = new();

        // Add a candlestick series to the plot
        myPlot.Title($"{symbol} - [{beginDate.ToString("yyyy-MM-dd")} - {endDate.ToString("yyyy-MM-dd")}]");
        myPlot.Add.Candlestick(data);
        myPlot.Axes.DateTimeTicksBottom();

        // Get the image bytes
        byte[] imageBytes = myPlot.GetImageBytes(5000, 500, ScottPlot.ImageFormat.Png);

        var result = new BinaryFile
        {
            MimeType = "image/svg",
            FileName = $"{symbol}_{beginDate:yyyyMMdd}_{endDate:yyyyMMdd}.png",
            FileContent = imageBytes,
        };

        return result;
    }
}
