using CurrencyConverter.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;
        private readonly IExchangeRateProvider _rateProvider;

        public CurrencyController(
            ICurrencyConverter converter,
            IExchangeRateProvider rateProvider)
        {
            _converter = converter;
            _rateProvider = rateProvider;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert(
            [FromQuery] string fromCurrency,
            [FromQuery] string toCurrency,
            [FromQuery] decimal amount)
        {
            try
            {
                var result = await _converter.ConvertAsync(fromCurrency, toCurrency, amount);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("rates/latest")]
        public async Task<IActionResult> GetLatestRate(
            [FromQuery] string sourceCurrency,
            [FromQuery] string targetCuurency)
        {
            var rate = await _rateProvider.GetLatestRateAsync(sourceCurrency, targetCuurency);
            return Ok(rate);
        }
    }
}
