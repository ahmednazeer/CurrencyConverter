using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Services;
using CurrencyConverter.Infrastructure.Decorators;
using CurrencyConverter.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Text;

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

builder.Services.AddHttpClient("Frankfurter")
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddTransientHttpErrorPolicy(policy =>
        policy.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Basic", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
