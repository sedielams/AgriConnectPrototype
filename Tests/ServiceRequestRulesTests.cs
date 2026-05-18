using AgriConnectPrototype.Services;

namespace AgriConnectPrototype.Tests
{
    // Tests business rules for service requests
    // Requests should be blocked for Expired or On Hold contracts

    public class ServiceRequestRulesTests
    {
        [Theory]

        // Test multiple contract status values
        [InlineData("Active", true)]
        [InlineData("Draft", true)]
        [InlineData("Expired", false)]
        [InlineData("On Hold", false)]

        public void CanCreateServiceRequest_ShouldReturnExpectedResult(
            string status,
            bool expected)
        {
            // Create rules object
            var rules = new ServiceRequestRules();

            // Test contract status
            var result =
                rules.CanCreateServiceRequest(status);

            // Verify expected result
            Assert.Equal(expected, result);
        }
    }
}