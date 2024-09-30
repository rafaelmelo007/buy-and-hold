namespace BuyAndHold.Api.Common.Extensions;
public static class CandlesExtension
{
    public static Dictionary<DateOnly, Candle> ToCandles(this IEnumerable<DailyStockPrice> rows)
    {
        var orderedRows = rows.OrderBy(x => x.Date).ToList();
        var candles = new List<Candle>();

        for (int i = 0; i < orderedRows.Count; i++)
        {
            var currentRow = orderedRows[i];

            var average21 = i >= 20 ? orderedRows.Skip(i - 20).Take(21).Average(x => x.Close) : (double?)null;
            var average50 = i >= 49 ? orderedRows.Skip(i - 49).Take(50).Average(x => x.Close) : (double?)null;
            var average200 = i >= 199 ? orderedRows.Skip(i - 199).Take(200).Average(x => x.Close) : (double?)null;

            var candle = new Candle
            {
                Open = currentRow.Open,
                Close = currentRow.Close,
                High = currentRow.High,
                Low = currentRow.Low,
                Symbol = currentRow.Symbol,
                Average21 = average21,
                Average50 = average50,
                Average200 = average200,
                Date = currentRow.Date,
                Volume = currentRow.Volume ?? double.NegativeZero
            };

            candles.Add(candle);
        }

        var result = candles.ToDictionary(x => x.Date, x => x);
        return result;
    }


}
