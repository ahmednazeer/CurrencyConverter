using CurrencyConverter.API.Controllers;
using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CurrencyConverter.API.Tests
{
    public class CurrencyControllerTests
    {
        [Fact]
        public async Task Convert_ShouldReturnOkWithConversionResult()
        {
            // Arrange
            var mockConverter = new Mock<ICurrencyConverter>();
            mockConverter.Setup(x => x.ConvertAsync("USD", "EUR", 100))
                        .ReturnsAsync(new ConversionResult
                        {
                            Amount = 100,
                            ConvertedAmount = 85,
                            FromCurrency = "USD",
                            ToCurrency = "EUR",
                            Rate = 0.85m
                        });

            var mockProvider = new Mock<IExchangeRateProvider>();
            var controller = new CurrencyController(mockConverter.Object, mockProvider.Object);

            // Act
            var result = await controller.Convert("USD", "EUR", 100);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var conversionResult = Assert.IsType<ConversionResult>(okResult.Value);
            Assert.Equal(85, conversionResult.ConvertedAmount);
        }

        [Fact]
        public async Task Convert_ShouldReturnBadRequestForRestrictedCurrencies()
        {
            // Arrange
            var mockConverter = new Mock<ICurrencyConverter>();
            mockConverter.Setup(x => x.ConvertAsync("TRY", "EUR", 100))
                        .ThrowsAsync(new ArgumentException("Invalid currency"));

            var mockProvider = new Mock<IExchangeRateProvider>();
            var controller = new CurrencyController(mockConverter.Object, mockProvider.Object);

            // Act
            var result = await controller.Convert("TRY", "EUR", 100);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetLatestRate_ShouldReturnExchangeRate()
        {
            // Arrange
            var mockConverter = new Mock<ICurrencyConverter>();
            var mockProvider = new Mock<IExchangeRateProvider>();
            mockProvider.Setup(x => x.GetLatestRateAsync("USD", "EUR"))
                       .ReturnsAsync(new ExchangeRate() { BaseCurrency = "USD", TargetCurrency = "EUR", Rate = 0.85m, Date = DateTime.UtcNow });

            var controller = new CurrencyController(mockConverter.Object, mockProvider.Object);

            // Act
            var result = await controller.GetLatestRate("USD", "EUR");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rate = Assert.IsType<ExchangeRate>(okResult.Value);
            Assert.Equal(0.85m, rate.Rate);
        }
    }
}