namespace BuyAndHold.Api.MarketBreadth.Services;
public interface IMarketBreadthService
{
    Task<IEnumerable<AverageLines>> GetWalletBreadthAsync(long walletId, CancellationToken cancellationToken);
}
