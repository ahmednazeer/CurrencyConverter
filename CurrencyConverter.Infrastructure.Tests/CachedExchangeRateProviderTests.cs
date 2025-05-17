using CurrencyConverter.Core.Contracts;
using CurrencyConverter.Core.Models;
using CurrencyConverter.Infrastructure.Decorators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Tests
{
    //tests needs to revisted
    public class CachedExchangeRateProviderTests
    {
        [Fact]
        public async Task GetLatestRateAsync_ShouldReturnCachedValueWhenAvailable()
        {
            // Arrange
            var mockProvider = new Mock<IExchangeRateProvider>();
            var mockCache = new Mock<IDistributedCache>();

            var cachedRate = new ExchangeRate() { BaseCurrency = "USD", TargetCurrency = "EUR", Rate = 0.85m, Date = DateTime.UtcNow };
            var serializedRate = JsonSerializer.Serialize(cachedRate);
            mockCache.Setup(x => x.GetStringAsync(It.IsAny<string>(), default))
                    .ReturnsAsync(serializedRate);

            var logger = Mock.Of<ILogger<CachedExchangeRateProvider>>();
            var cachedProvider = new CachedExchangeRateProvider(
                mockProvider.Object, mockCache.Object, logger);

            // Act
            var result = await cachedProvider.GetLatestRateAsync("USD", "EUR");

            // Assert
            Assert.Equal(0.85m, result.Rate);
            mockProvider.Verify(
                x => x.GetLatestRateAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task GetLatestRateAsync_ShouldCacheNewValues()
        {
            // Arrange
            var mockProvider = new Mock<IExchangeRateProvider>();
            var mockCache = new Mock<IDistributedCache>();

            var freshRate = new ExchangeRate() { BaseCurrency = "USD", TargetCurrency = "EUR", Rate = 0.85m, Date = DateTime.UtcNow };
            mockProvider.Setup(x => x.GetLatestRateAsync("USD", "EUR"))
                       .ReturnsAsync(freshRate);

            mockCache.Setup(x => x.GetStringAsync(It.IsAny<string>(), default))
                    .ReturnsAsync((string)null);

            var logger = Mock.Of<ILogger<CachedExchangeRateProvider>>();
            var cachedProvider = new CachedExchangeRateProvider(
                mockProvider.Object, mockCache.Object, logger);

            // Act
            var result = await cachedProvider.GetLatestRateAsync("USD", "EUR");

            // Assert
            mockCache.Verify(x => x.SetStringAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                default), Times.Once);
        }
    }
}
