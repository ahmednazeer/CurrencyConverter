using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Services
{
    public class FrankfurterApiClient : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FrankfurterApiClient> _logger;

        public FrankfurterApiClient(
            HttpClient httpClient,
            ILogger<FrankfurterApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://api.frankfurter.app/");
        }

        public async Task<ExchangeRate> GetLatestRateAsync(
            string baseCurrency,
            string targetCurrency)
        {
            var response = await _httpClient.GetAsync($"latest?from={baseCurrency}&to={targetCurrency}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<FrankfurterResponse>();
            return new ExchangeRate
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = targetCurrency,
                Rate = content.Rates[targetCurrency],
                Date = content.Date
            };
        }
    }


}
