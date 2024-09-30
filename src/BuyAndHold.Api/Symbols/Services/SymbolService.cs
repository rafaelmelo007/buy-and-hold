namespace BuyAndHold.Api.Symbols.Services;
public class SymbolService : ISymbolService
{
    private readonly BuyAndHoldDbContext _database;
    private IDapperConnectionFactory _connectionFactory;

    public SymbolService(
        BuyAndHoldDbContext database,
        IDapperConnectionFactory connectionFactory)
    {
        _database = database;
        _connectionFactory = connectionFactory;
    }

    public async Task<Dictionary<string, double>?> GetSymbolPricesAsync(CancellationToken cancellationToken)
    {
        var prices = await _database.Symbols.ToDictionaryAsync(x => x.Name!, x => x.LastPrice.Value);
        return prices;
    }

    public async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
    {
        var symbols = _database.Symbols.ToList();
        using var conn = _connectionFactory.Create(default, default);
        var score = conn.QueryRawSql<Symbol>("select Name, BuyMood, SellMood from vwSymbols WITH (NOLOCK)").ToDictionary(x => (string)x.Name, x => x);
        foreach (var symbol in symbols)
        {
            if (!score.ContainsKey(symbol.Name)) continue;

            symbol.BuyMood = score[symbol.Name].BuyMood;
            symbol.SellMood = score[symbol.Name].SellMood;
        }
        return symbols;
    }
}
