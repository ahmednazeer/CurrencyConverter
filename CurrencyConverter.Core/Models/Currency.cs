using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Models
{
    public class Currency
    {
        public string Code { get; set; }  // ISO code like USD, EUR
        public string Name { get; set; }
    }
}
