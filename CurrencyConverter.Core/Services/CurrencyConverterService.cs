using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Services
{
    public class CurrencyConverterService : ICurrencyConverter
    {
        private readonly IExchangeRateProvider _rateProvider;
        private readonly ILogger<CurrencyConverterService> _logger;

        public CurrencyConverterService(
            IExchangeRateProvider rateProvider,
            ILogger<CurrencyConverterService> logger)
        {
            _rateProvider = rateProvider;
            _logger = logger;
        }

        public async Task<ConversionResult> ConvertAsync(
            string fromCurrency,
            string toCurrency,
            decimal amount)
        {
            // Validate restricted currencies
            var restrictedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };
            if (restrictedCurrencies.Contains(fromCurrency) ||
                restrictedCurrencies.Contains(toCurrency))
            {
                _logger.LogWarning(
                    "Attempt to convert restricted currency: {From} to {To}",
                    fromCurrency, toCurrency);
                throw new ArgumentException(
                    $"Currency conversion not supported for {fromCurrency} or {toCurrency}");
            }

            var rate = await _rateProvider.GetLatestRateAsync(fromCurrency, toCurrency);

            return new ConversionResult
            {
                Amount = amount,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ConvertedAmount = amount * rate.Rate,
                Rate = rate.Rate,
                RateDate = rate.Date
            };
        }
    }
}
