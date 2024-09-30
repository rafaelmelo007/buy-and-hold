using System.Globalization;
using System.Text;

namespace BuyAndHold.Core.Renders;
public class HtmlChartBuilder : IChartImageBuilder
{
    public BinaryFile? Build(Chart chart, StrategyResults? strategyResults = default)
    {
        var template = ResourcesHelper.ToString("Resources.Chart.html");
        var culture = new CultureInfo("en-US");
        var data = chart.Candles.Select(x =>
        {
            var date = x.Value.Date.ToString("yy-MM-dd");
            var o = x.Value.Open.ToString("#.00", culture);
            var h = x.Value.High.ToString("#.00", culture);
            var l = x.Value.Low.ToString("#.00", culture);
            var c = x.Value.Close.ToString("#.00", culture);
            return new { date, o, h, l, c };
        }).ToList();


        var json = string.Join("\r\n", data.Select(x =>
        {
            var content = string.Empty;
            content = $"{{date: '{x.date}', open: {x.o}, high: {x.h}, low: {x.l}, close: {x.c}, gain: 0}},";
            return content;
        }));

        var content = template.Replace("#DATA_HERE#", json);
        var result = new BinaryFile
        {
            MimeType = "text/html",
            FileName = $"{chart.Symbol}_{chart.Begin:yyyyMMdd}_{chart.End:yyyyMMdd}.html",
            FileContent = Encoding.UTF8.GetBytes(content),
        };

        return result;
    }

    public BinaryFile? Build(string symbol, IEnumerable<StrategyResultCandle> candles)
    {
        var template = ResourcesHelper.ToString("Resources.Chart.html");
        var culture = new CultureInfo("en-US");
        var beginDate = candles.Min(x => x.Date);
        var endDate = candles.Max(x => x.Date);
        var data = candles.Select(x =>
        {
            var date = x.Date.ToString("yy-MM-dd");
            var o = x.Open.ToString("#.00", culture);
            var h = x.High.ToString("#.00", culture);
            var l = x.Low.ToString("#.00", culture);
            var c = x.Close.ToString("#.00", culture);
            var g = x.ProfitPercentage.ToString("#.00", culture);
            return new { date, o, h, l, c, g };
        }).ToList();


        var json = string.Join("\r\n", data.Select(x =>
        {
            var content = string.Empty;
            content = $"{{date: '{x.date}', open: {x.o}, high: {x.h}, low: {x.l}, close: {x.c}, gain: {x.g}}},";
            return content;
        }));

        var content = template.Replace("#DATA_HERE#", json);
        var result = new BinaryFile
        {
            MimeType = "text/html",
            FileName = $"{symbol}_{beginDate:yyyyMMdd}_{endDate:yyyyMMdd}.html",
            FileContent = Encoding.UTF8.GetBytes(content),
        };

        return result;
    }
}
