using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MadkassenRestAPI.Services;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class WeatherServiceTests
    {
        private class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _content;

            public StubHttpMessageHandler(HttpStatusCode statusCode, string content)
            {
                _statusCode = statusCode;
                _content = content;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_content, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }

        private WeatherService CreateService(HttpStatusCode statusCode, string json)
        {
            var handler = new StubHttpMessageHandler(statusCode, json);
            var client = new HttpClient(handler);

            return new WeatherService(client);
        }


        [Theory]
        [InlineData(55.4, 12.45, "{\"current_weather\":{\"temperature\":5.9,\"windspeed\":28.1}}", 5.9, 28.1)]
        [InlineData(40.7, -74.0, "{\"current_weather\":{\"temperature\":10.0,\"windspeed\":3.5}}", 10.0, 3.5)]
        public async Task GetCurrentWeatherAsync_ValidResponse_ReturnsResult(
            double lat,
            double lon,
            string json,
            double expectedTemp,
            double expectedWind)
        {
            // ARRANGE
            var service = CreateService(HttpStatusCode.OK, json);

            // ACT
            var result = await service.GetCurrentWeatherAsync(lat, lon);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(expectedTemp, result!.Temperature);
            Assert.Equal(expectedWind, result.WindSpeed);
        }


        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task GetCurrentWeatherAsync_NonSuccessStatus_ReturnsNull(HttpStatusCode statusCode)
        {
            // ARRANGE
            var service = CreateService(statusCode, "{\"current_weather\":{\"temperature\":5.0,\"windspeed\":1.0}}");

            // ACT
            var result = await service.GetCurrentWeatherAsync(55.4, 12.45);

            // ASSERT
            Assert.Null(result);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"current_weather\":{}}")]
        [InlineData("{\"current_weather\":{\"temperature\":5.0}}")]     // mangler windspeed
        [InlineData("{\"current_weather\":{\"windspeed\":3.0}}")]      // mangler temperature
        public async Task GetCurrentWeatherAsync_InvalidJson_ReturnsNull(string json)
        {
            // ARRANGE
            var service = CreateService(HttpStatusCode.OK, json);

            // ACT
            var result = await service.GetCurrentWeatherAsync(55.4, 12.45);

            // ASSERT
            Assert.Null(result);
        }
    }
}
