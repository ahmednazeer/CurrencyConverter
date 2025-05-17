using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using CurrencyConverter.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Tests
{
    public class CurrencyConverterServiceTests
    {
        [Fact]
        public async Task ConvertAsync_ShouldReturnCorrectConversion()
        {
            // Arrange
            var mockProvider = new Mock<IExchangeRateProvider>();
            mockProvider.Setup(x => x.GetLatestRateAsync("USD", "EUR"))
                       .ReturnsAsync(new ExchangeRate() { BaseCurrency = "USD", TargetCurrency = "EUR", Rate = 0.85m, Date = DateTime.UtcNow });

            var logger = Mock.Of<ILogger<CurrencyConverterService>>();
            var service = new CurrencyConverterService(mockProvider.Object, logger);

            // Act
            var result = await service.ConvertAsync("USD", "EUR", 100);

            // Assert
            Assert.Equal(85, result.ConvertedAmount);
            Assert.Equal(0.85m, result.Rate);
        }

        [Theory]
        [InlineData("TRY", "EUR")]
        [InlineData("EUR", "PLN")]
        [InlineData("THB", "MXN")]
        public async Task ConvertAsync_ShouldRejectRestrictedCurrencies(string from, string to)
        {
            // Arrange
            var mockProvider = new Mock<IExchangeRateProvider>();
            var logger = Mock.Of<ILogger<CurrencyConverterService>>();
            var service = new CurrencyConverterService(mockProvider.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.ConvertAsync(from, to, 100));
        }

        [Fact]
        public async Task ConvertAsync_ShouldLogWarningForRestrictedCurrencies()
        {
            // Arrange
            var mockProvider = new Mock<IExchangeRateProvider>();
            var mockLogger = new Mock<ILogger<CurrencyConverterService>>();
            var service = new CurrencyConverterService(mockProvider.Object, mockLogger.Object);

            // Act
            try { await service.ConvertAsync("TRY", "EUR", 100); }
            catch { /* Expected */ }

            // Assert
            mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Attempt to convert restricted currency")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
