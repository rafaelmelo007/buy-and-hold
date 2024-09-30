namespace BuyAndHold.Api.Wallets.Services;
public interface IWalletsService
{
    Task<IEnumerable<Wallet>> GetWalletsWithSymbolsAsync();
    Task<long> RemoveWalletAsync(long walletId,
        CancellationToken cancellationToken);
    Task<long> AddOrUpdateWalletAsync(Wallet wallet,
        CancellationToken cancellationToken);
    Task<Wallet?> GetWalletWithSymbolsAsync(long walletId,
        CancellationToken cancellationToken);
}
