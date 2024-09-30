using BuyAndHold.Api.Authentication;
using BuyAndHold.Api.Charts;
using BuyAndHold.Api.MarketBreadth;
using BuyAndHold.Api.MarketBreadth.Services;
using BuyAndHold.Api.MarketMood;
using BuyAndHold.Api.Symbols;
using BuyAndHold.Api.Wallets;
using dotenv.net;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();
builder.Services.AddTransient<BuyAndHold.Api.Common.Services.ILogger, SerilogLogger>();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IDapperConnectionFactory, DapperConnectionFactory>();
builder.Services.AddTransient<IImportUrlService, ImportUrlService>();
builder.Services.AddTransient<ISymbolService, SymbolService>();
builder.Services.AddTransient<IJobsService, JobsService>();
builder.Services.AddTransient<IChartService, ChartService>();
builder.Services.AddTransient<IWalletsService, WalletsService>();
builder.Services.AddTransient<IMarketBreadthService, MarketBreadthService>();
builder.Services.AddPersistence(builder.Configuration, false);

builder.Services.Configure<VendorsConfiguration>(builder.Configuration.GetSection(nameof(VendorsConfiguration)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Buy And Hold Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});

builder.Services.AddAuthentication().AddBearerToken();
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Replace with your frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapGroup("/")
    .WithTags("Health Check")
    .MapGet("/health-check",
    () => new { message = "OK" })
    .WithSummary("Check if the api is up and running");

app.MapAuthenticationEndpoints();
app.MapDailyStockPricesEndpoints();
app.MapSymbolsEndpoints();
app.MapMarketMoodEndpoints();
app.MapChartsEndpoints();
app.MapMarketBreadthEndpoints();
app.MapWalletsEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.Run();


