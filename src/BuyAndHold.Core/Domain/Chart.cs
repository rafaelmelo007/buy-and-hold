using System.Globalization;
using System.Text;

namespace BuyAndHold.Core.Domain;
public record Chart
{
    public string Symbol { get; init; }
    public Dictionary<DateOnly, Candle> Candles { get; init; }
    public DateOnly Begin { get; set; }
    public DateOnly End { get; set; }
    public DateOnly? LastPriceDate => Candles.OrderByDescending(x => x.Key).FirstOrDefault().Value.Date;
    public double? LastPrice => Candles.OrderByDescending(x => x.Key).FirstOrDefault().Value.Close;

    public double? GetHighFromPeriod(DateOnly begin, DateOnly end)
    {
        var high = Candles.Where(x => x.Value.Date >= begin && x.Value.Date <= end).Max(x => x.Value.High);
        return high;
    }

    public double? GetLowFromPeriod(DateOnly begin, DateOnly end)
    {
        var low = Candles.Where(x => x.Value.Date >= begin && x.Value.Date <= end).Min(x => x.Value.Low);
        return low;
    }

    public static Chart FromChartFile(string symbol, string filePath)
    {
        var culture = new CultureInfo("en-US");
        var lines = File.ReadAllLines(filePath);
        List<string> header = null;
        var chart = new Chart
        {
            Symbol = symbol,
            Candles = []
        };
        foreach (var line in lines)
        {
            if (header is null)
            {
                header = line.Split(';').ToList();
                continue;
            }

            var fields = line.Split(';');
            var date = DateOnly.Parse(fields[header.IndexOf("date")]);
            var o = double.Parse(fields[header.IndexOf("o")], culture);
            var c = double.Parse(fields[header.IndexOf("c")], culture);
            var h = double.Parse(fields[header.IndexOf("h")], culture);
            var l = double.Parse(fields[header.IndexOf("l")], culture);
            var a = double.Parse(fields[header.IndexOf("a")], culture);
            chart.Candles.Add(date, new Candle
            {
                Date = date,
                Open = o,
                Close = c,
                High = h,
                Low = l,
                Symbol = symbol,
            });
        }
        chart.Begin = chart.Candles.Keys.OrderBy(x => x).First();
        chart.End = chart.Candles.Keys.OrderBy(x => x).Last();
        return chart;
    }

    public BinaryFile ToChartFile()
    {
        var culture = new CultureInfo("en-US"); // You can replace "en-US" with your desired culture
        var sb = new StringBuilder();

        sb.AppendLine($"date;o;c;h;l;a");
        foreach (var candle in Candles)
        {
            var cd = candle.Value;

            sb.AppendLine($"{cd.Date.ToString("yyyy-MM-dd", culture)};" +
                          $"{cd.Open.ToString("#.00", culture)};" +
                          $"{cd.Close.ToString("#.00", culture)};" +
                          $"{cd.High.ToString("#.00", culture)};" +
                          $"{cd.Low.ToString("#.00", culture)};0");
        }

        var file = new BinaryFile
        {
            FileName = $"{Symbol}__{Begin:yyyyMMdd}__{End:yyyyMMdd}.candleschart",
            MimeType = "application/octet-stream",
            FileContent = Encoding.UTF8.GetBytes(sb.ToString())
        };

        return file;
    }
}
