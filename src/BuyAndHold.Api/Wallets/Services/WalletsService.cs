namespace BuyAndHold.Api.Wallets.Services;
public class WalletsService : IWalletsService
{
    private readonly BuyAndHoldDbContext _database;
    private readonly ICurrentUserService _currentUserService;

    public WalletsService(BuyAndHoldDbContext database,
        ICurrentUserService currentUserService)
    {
        _database = database;
        _currentUserService = currentUserService;
    }


    public async Task<IEnumerable<Wallet>> GetWalletsWithSymbolsAsync()
    {
        var userId = _currentUserService.UserId!.Value;
        var wallets = await _database.Wallets.Where(x => x.UserId == userId).Include(x => x.Symbols).ToListAsync();
        return wallets;
    }

    public async Task<long> AddOrUpdateWalletAsync(
        Wallet wallet, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        var row = await _database.Wallets.SingleOrDefaultAsync(x => x.UserId == userId && x.WalletId == wallet.WalletId, cancellationToken);
        if (row is null)
        {
            row = wallet;
            row.WalletId = 0;
            await _database.Wallets.AddAsync(row, cancellationToken);
        }
        row.UserId = userId;
        row.Name = wallet.Name;
        await _database.SaveChangesAsync(cancellationToken);

        if (wallet.Symbols != null && wallet.Symbols.Any())
        {
            foreach (var symbol in wallet.Symbols)
            {
                var rowSymbol = await _database.WalletSymbols.SingleOrDefaultAsync(
                    x => x.WalletId == row.WalletId && x.Symbol == symbol.Symbol, cancellationToken);
                if (rowSymbol is null)
                {
                    rowSymbol = new WalletSymbol();
                    await _database.WalletSymbols.AddAsync(rowSymbol, cancellationToken);
                }

                rowSymbol.WalletId = row.WalletId;
                rowSymbol.Symbol = symbol.Symbol;
                rowSymbol.Quantity = symbol.Quantity;
                rowSymbol.AveragePrice = symbol.AveragePrice;
                rowSymbol.ExpectedQuantity = symbol.ExpectedQuantity;
            }

            var symbolNames = wallet.Symbols.Select(x => x.Symbol).ToList();
            var deleted = await _database.WalletSymbols.Where(x => x.WalletId == row.WalletId && !symbolNames.Contains(x.Symbol)).ToListAsync();
            _database.WalletSymbols.RemoveRange(deleted);
        }

        await _database.SaveChangesAsync(cancellationToken);

        return row.WalletId;
    }

    public async Task<long> RemoveWalletAsync(long walletId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        var wallets = await _database.Wallets.Where(x => x.WalletId == walletId && x.UserId == userId).ToListAsync();
        var symbols = await _database.WalletSymbols.Where(x => x.WalletId == walletId && x.Wallet.UserId == userId).ToListAsync();
        _database.WalletSymbols.RemoveRange(symbols);
        _database.Wallets.RemoveRange(wallets);
        var affected = await _database.SaveChangesAsync(cancellationToken);
        return affected;
    }

    public async Task<Wallet?> GetWalletWithSymbolsAsync(long walletId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        var wallet = await _database.Wallets.Where(x => x.WalletId == walletId && x.UserId == userId)
            .Include(x => x.Symbols).FirstOrDefaultAsync(cancellationToken);
        return wallet;
    }
}
