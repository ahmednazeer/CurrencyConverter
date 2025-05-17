using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Models
{
    public class ConversionResult
    {
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime RateDate { get; set; }
    }
}
