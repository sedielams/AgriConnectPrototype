using System.ComponentModel.DataAnnotations;

namespace AgriConnectPrototype.Models
{
    // Stores service requests linked to a farmer's supply contract.
    // Examples: transport, imported fertilizer, equipment or logistics.

    public class FarmServiceRequest
    {
        // Unique service request ID
        public int FarmServiceRequestId { get; set; }

        // Foreign key linking request to a supply contract
        [Required]
        public int SupplyContractId { get; set; }

        // Navigation property for related contract
        public SupplyContract? SupplyContract { get; set; }

        // Description of requested service
        [Required]
        public string Description { get; set; }

        // Amount entered in USD
        [Required]
        public decimal UsdAmount { get; set; }

        // Exchange rate retrieved from external API
        public decimal ExchangeRate { get; set; }

        // Final converted amount in South African Rand
        public decimal CostInZar { get; set; }

        // Current request status
        [Required]
        public string Status { get; set; }
    }
}