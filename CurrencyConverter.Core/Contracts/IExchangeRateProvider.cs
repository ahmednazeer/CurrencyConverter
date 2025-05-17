using CurrencyConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Contracts
{
    public interface IExchangeRateProvider
    {
        Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency);
        Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(
            string baseCurrency,
            DateTime startDate,
            DateTime endDate,
            int page = 1,
            int pageSize = 10);
    }
}
