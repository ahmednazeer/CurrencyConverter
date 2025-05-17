using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Decorators
{
    public class CachedExchangeRateProvider : IExchangeRateProvider
    {
        private readonly IExchangeRateProvider _rateProvider;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedExchangeRateProvider> _logger;

        public CachedExchangeRateProvider(
            IExchangeRateProvider innerProvider,
            IDistributedCache cache,
            ILogger<CachedExchangeRateProvider> logger)
        {
            _rateProvider = innerProvider;
            _cache = cache;
            _logger = logger;
        }

        public Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<ExchangeRate> GetLatestRateAsync(
            string baseCurrency,
            string targetCurrency)
        {
            var cacheKey = $"rate_{baseCurrency}_{targetCurrency}";
            var cachedRate = await _cache.GetStringAsync(cacheKey);

            if (cachedRate != null)
            {
                return JsonSerializer.Deserialize<ExchangeRate>(cachedRate);
            }

            var rate = await _rateProvider.GetLatestRateAsync(baseCurrency, targetCurrency);
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(rate),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

            return rate;
        }
    }
}
