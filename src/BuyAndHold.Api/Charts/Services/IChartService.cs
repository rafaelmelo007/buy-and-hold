
namespace BuyAndHold.Api.Charts.Services;
public interface IChartService
{
    Task<Chart> GetChartAsync(string symbol, DateOnly begin, DateOnly end, CancellationToken cancellationToken);
    Task<Chart> GetWalletChartAsync(long wallertId, DateOnly begin, DateOnly end, CancellationToken cancellationToken);
}
