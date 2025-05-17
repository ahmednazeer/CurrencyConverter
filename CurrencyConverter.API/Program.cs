using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Services;
using CurrencyConverter.Infrastructure.Decorators;
using CurrencyConverter.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverterService>();
builder.Services.AddScoped<IExchangeRateProvider>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    var logger = provider.GetRequiredService<ILogger<FrankfurterApiClient>>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    var cacheLogger = provider.GetRequiredService<ILogger<CachedExchangeRateProvider>>();

    var frankfurterService = new FrankfurterApiClient(httpClient, logger);
    return new CachedExchangeRateProvider(frankfurterService, cache, cacheLogger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
