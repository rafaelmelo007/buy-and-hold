using System.Web;

namespace BuyAndHold.Api.DailyStockPrices.Services;
public class ImportUrlService : IImportUrlService
{
    private readonly BuyAndHoldDbContext _database;
    private readonly IDateTimeService _dateTimeService;

    public ImportUrlService(
        BuyAndHoldDbContext database,
        IDateTimeService dateTimeService)
    {
        _database = database;
        _dateTimeService = dateTimeService;
    }

    public async Task<string?> GetHttpRequestAsync(string url, string symbol)
    {
        var row = await _database.ImportedUrls.FirstOrDefaultAsync(x => x.Url == url);
        if (row is not null)
        {
            return row.Content;
        }

        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);

        var importRow = new ImportedUrl
        {
            Url = url,
            Content = content,
            CreatedAt = _dateTimeService.NowUtc
        };
        _database.ImportedUrls.Add(importRow);
        await _database.SaveChangesAsync();

        var symbolRow = await _database.Symbols.FirstOrDefaultAsync(x => x.Name == symbol);
        if (symbolRow is null)
        {
            symbolRow = new Symbol
            {
                Name = symbol,
                ImportedUrlId = importRow.Id,
                LastUpdatedAt = _dateTimeService.NowUtc
            };
            _database.Symbols.Add(symbolRow);
        }
        else
        {
            symbolRow.ImportedUrlId = importRow.Id;
            symbolRow.LastUpdatedAt = _dateTimeService.NowUtc;
        }

        await _database.SaveChangesAsync();

        return content;
    }

    public IEnumerable<string> ListAllSymbolsFromUrl(string url)
    {
        var urls = _database.ImportedUrls.Where(x => x.Url.StartsWith(url)).Select(x => x.Url).ToList();
        foreach (var urlItem in urls)
        {
            var symbol = urlItem.Replace("https://query1.finance.yahoo.com/v7/finance/download/", string.Empty);
            symbol = symbol.Substring(0, symbol.IndexOf("?"));
            yield return HttpUtility.UrlDecode(symbol);
        }
    }
}
