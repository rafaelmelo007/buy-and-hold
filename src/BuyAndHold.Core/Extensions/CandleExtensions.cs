using System.Text;

namespace BuyAndHold.Core.Extensions;
public static class CandleExtensions
{
    // Extension method to check if the prices are falling
    public static bool ArePricesFalling(this List<Candle> candles)
    {
        if (!candles.All(x => x.Open > x.Close)) return false;

        for (int i = 1; i < candles.Count; i++)
        {
            if (candles[i - 1].Close <= candles[i].Close &&
                candles[i - 1].Open <= candles[i].Open)
            {
                return false;
            }
        }
        return true;
    }

    // Extension method to check if the body of each candle is valid
    public static bool AreCandleBodiesValid(this List<Candle> candles)
    {
        foreach (var candle in candles)
        {
            var body = Math.Abs(candle.Open - candle.Close);
            var range = candle.High - candle.Low;
            if (range == 0 || body / range <= Convert.ToDouble(0.6))
                return false;
        }
        return true;
    }

    // Extension method to calculate the fall amount
    public static double CalculateMovePercentage(this List<Candle> candles)
    {
        var beginPrice = candles.Last().Open;
        var endPrice = candles.First().Close;
        // Calculate the fall amount
        var fallAmount = beginPrice - endPrice;
        // Calculate the percentage fall amount
        var result = (fallAmount / beginPrice) * 100;
        return result;
    }

    // Extension method to check if the prices are rising
    public static bool ArePricesRising(this List<Candle> candles)
    {
        for (int i = 1; i < candles.Count; i++)
        {
            if (candles[i - 1].Close >= candles[i].Close)
                return false;
        }
        return true;
    }

    public static string ToHtmlChart(this Chart chart)
    {
        var template = ResourcesHelper.ToString("Charts.Resources.Chart.html");
        var data = chart.Candles.Select(x =>
        {
            var date = x.Key.ToString("yy-MM-dd");
            var o = (int)(x.Value.Open * 100);
            var h = (int)(x.Value.High * 100);
            var l = (int)(x.Value.Low * 100);
            var c = (int)(x.Value.Close * 100);
            return new { date, o, h, l, c };
        }).ToList();


        var json = string.Join("\r\n", data.Select(x =>
        {
            var content = string.Empty;
            content = $"{{date: '{x.date}', open: {x.o}, high: {x.h}, low: {x.l}, close: {x.c}}},";
            return content;
        }));

        return template.Replace("#DATA_HERE#", json);
    }

    public static IEnumerable<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        for (int i = 0; i < source.Count; i += chunkSize)
        {
            yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
        }
    }

    public static string Dump(this StrategyResults results)
    {
        var sb = new StringBuilder();
        foreach (var candle in results.Candles)
        {
            sb.AppendLine($"dt:{candle.Date:yyyy-MM-dd}\t\twa:{candle.AmountInWallet:#.00}\t\tcu:{candle.AmountInCustody:#.00}\t\tqt:{candle.QuantityInCustody}\t\tpr:{candle.Open:#.00}\t\tap:{candle.AveragePrice:#.00}\t\top:{candle.OrderExecuted?.OpportunityInfo}");
        }
        return sb.ToString();
    }
}