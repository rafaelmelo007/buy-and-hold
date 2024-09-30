namespace BuyAndHold.Core;
public class TradeStrategyFactory : IInvestStrategyFactory
{
    private IEnumerable<IInvestStrategy> _strategies;

    public TradeStrategyFactory(IEnumerable<IInvestStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IInvestStrategy? Create(string name, string? settings)
    {
        var strategy = _strategies.FirstOrDefault(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());
        if (string.IsNullOrWhiteSpace(settings)) return strategy;
        strategy?.LoadSettingsFromJson(settings);
        return strategy;
    }

    public IInvestStrategy? CreateDefault()
    {
        return _strategies.FirstOrDefault();
    }

    public List<IInvestStrategyFactory.StrategyType>? ListStrategyTypes()
    {
        var result = _strategies.Select(x => new IInvestStrategyFactory.StrategyType(x.Name, x.GetSettingsJson())).ToList();
        return result;
    }
}
