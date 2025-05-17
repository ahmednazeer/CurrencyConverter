using CurrencyConverter.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace CurrencyConverter.Infrastructure.Tests
{
    public class FrankfurterApiClientTests
    {
        [Fact]
        public async Task GetLatestRateAsync_ShouldReturnCorrectExchangeRate()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://api.frankfurter.app/latest?from=USD&to=EUR")
                   .Respond("application/json",
                       "{\"amount\":1,\"base\":\"USD\",\"date\":\"2023-01-01\",\"rates\":{\"EUR\":0.85}}");

            var client = new HttpClient(mockHttp);
            var logger = Mock.Of<ILogger<FrankfurterApiClient>>();
            var apiClient = new FrankfurterApiClient(client, logger);

            // Act
            var result = await apiClient.GetLatestRateAsync("USD", "EUR");

            // Assert
            Assert.Equal("USD", result.BaseCurrency);
            Assert.Equal("EUR", result.TargetCurrency);
            Assert.Equal(0.85m, result.Rate);
            Assert.Equal(new DateTime(2023, 1, 1), result.Date);
        }

        [Fact]
        public async Task GetLatestRateAsync_ShouldThrowOnApiError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://api.frankfurter.app/*")
                   .Respond(HttpStatusCode.InternalServerError);

            var client = new HttpClient(mockHttp);
            var logger = Mock.Of<ILogger<FrankfurterApiClient>>();
            var apiClient = new FrankfurterApiClient(client, logger);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => apiClient.GetLatestRateAsync("USD", "EUR"));
        }
    }
}