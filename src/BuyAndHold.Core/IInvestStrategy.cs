namespace BuyAndHold.Core;
public interface IInvestStrategy
{
    string Name { get; }
    string Description { get; }
    string[] UsingSymbols { get; }
    string GetSettingsJson();
    bool LoadSettingsFromJson(string content);
    StrategyResults RunBackTest(string strategyName, string symbol,
        Dictionary<string, Chart> charts, WalletBalance wallet);
}
