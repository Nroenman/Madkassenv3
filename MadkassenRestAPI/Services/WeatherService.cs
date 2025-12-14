using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MadkassenRestAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResult?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            var url =
                $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // hvis top-level er et array, så brug første element
                if (root.ValueKind == JsonValueKind.Array)
                {
                    if (root.GetArrayLength() == 0)
                        return null;

                    root = root[0];
                }

                if (root.ValueKind != JsonValueKind.Object)
                    return null;

                if (!root.TryGetProperty("current_weather", out var currentWeather))
                    return null;

                if (!currentWeather.TryGetProperty("temperature", out var tempProp))
                    return null;

                if (!currentWeather.TryGetProperty("windspeed", out var windProp))
                    return null;

                var temperature = tempProp.GetDouble();
                var windSpeed = windProp.GetDouble();

                return new WeatherResult
                {
                    Temperature = temperature,
                    WindSpeed = windSpeed
                };
            }
            catch (JsonException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                // fx hvis vi prøver at læse array som object eller omvendt
                return null;
            }
        }
    }

    public class WeatherResult
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
    }
}
