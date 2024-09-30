using BuyAndHold.Core.Renders;
using BuyAndHold.Core.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace BuyAndHold.Core;
public static class Setup
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        //builder.Services.AddTransient<IChartImageBuilder, ScottPlotChartImageBuilder>();
        services.AddTransient<IChartImageBuilder, HtmlChartBuilder>();
        services.AddTransient<IInvestStrategyFactory, TradeStrategyFactory>();
        services.AddTransient<IInvestStrategy, BuyAndForgetStrategy>();
        services.AddTransient<IInvestStrategy, BrentHighLowStrategy>();
        services.AddTransient<IInvestStrategy, IbovHighLowStrategy>();
        services.AddTransient<IInvestStrategy, SymbolHighLowStrategy>();
        services.AddTransient<IInvestStrategy, PriceMovementStrategy>();
        return services;
    }
}
