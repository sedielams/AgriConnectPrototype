namespace AgriConnectPrototype.Services
{
    // Handles currency conversion for farming services.
    // Some farming products/services may be priced in USD.
    // The system converts these amounts to ZAR using the exchange rate API.

    public class CurrencyService
    {
        // Converts the entered USD amount to South African Rand (ZAR).
        // Example:
        // 100 USD × 18.50 = R1850

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return usdAmount * exchangeRate;
        }
    }
}