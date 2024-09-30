namespace BuyAndHold.Core.Renders;
public interface IChartImageBuilder
{
    BinaryFile? Build(Chart chart, StrategyResults? strategyResults = default);
    BinaryFile? Build(string symbol, IEnumerable<StrategyResultCandle> candles);
}
