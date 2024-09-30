
namespace BuyAndHold.Api.DailyStockPrices.Services;
public interface IImportUrlService
{
    Task<string?> GetHttpRequestAsync(string url, string symbol);
    IEnumerable<string> ListAllSymbolsFromUrl(string url);
}
