namespace BuyAndHold.Api.Symbols.Services;
public interface IJobsService
{
    Task<int> CalculateAveragesAsync(CancellationToken cancellationToken);
    Task<int> InspectOpportunitiesAsync(CancellationToken cancellationToken);
}
