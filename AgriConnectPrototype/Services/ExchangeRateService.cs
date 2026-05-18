using System.Text.Json;

namespace AgriConnectPrototype.Services
{
    // Connects to an external currency API.
    // The Farmers system uses this service to get the latest USD to ZAR rate.

    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        // Inject HttpClient for API communication
        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Gets the current USD → ZAR exchange rate
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            // Call external currency exchange API
            var response = await _httpClient.GetStringAsync(
                "https://open.er-api.com/v6/latest/USD");

            // Convert JSON response into readable data
            using var document = JsonDocument.Parse(response);

            // Extract the ZAR exchange rate
            var rate = document
                .RootElement
                .GetProperty("rates")
                .GetProperty("ZAR")
                .GetDecimal();

            // Return exchange rate
            return rate;
        }
    }
}