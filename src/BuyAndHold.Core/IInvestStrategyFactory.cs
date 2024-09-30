namespace BuyAndHold.Core;
public interface IInvestStrategyFactory
{
    public record StrategyType(string Name, string Settings);
    IInvestStrategy? Create(string name, string? settings);
    IInvestStrategy? CreateDefault();
    List<StrategyType>? ListStrategyTypes();
}
