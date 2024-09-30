namespace BuyAndHold.Api.Symbols.Services;
public interface ISymbolService
{
    Task<Dictionary<string, double>?> GetSymbolPricesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken);
}
