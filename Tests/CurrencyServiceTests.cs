using AgriConnectPrototype.Services;

namespace AgriConnectPrototype.Tests
{
    // Tests currency conversion logic
    // Verifies that USD values are converted correctly to ZAR

    public class CurrencyServiceTests
    {
        [Fact]

        // Test conversion calculation
        public void ConvertUsdToZar_ShouldReturnCorrectAmount()
        {
            // Create service object
            var service = new CurrencyService();

            // Test sample values
            // 100 USD × 18.50
            var result =
                service.ConvertUsdToZar(
                    100,
                    18.50m);

            // Expected result = R1850
            Assert.Equal(1850m, result);
        }
    }
}