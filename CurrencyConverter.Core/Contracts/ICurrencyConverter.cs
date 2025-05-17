using CurrencyConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Contracts
{
    public interface ICurrencyConverter
    {
        Task<ConversionResult> ConvertAsync(
            string fromCurrency,
            string toCurrency,
            decimal amount);
    }
}
