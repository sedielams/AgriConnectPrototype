using System.ComponentModel.DataAnnotations;

namespace AgriConnectPrototype.Models
{
    // Stores supply contracts for farmers.
    // Contracts define agreement details and control service request rules.

    public class SupplyContract
    {
        // Unique contract ID
        public int SupplyContractId { get; set; }

        // Foreign key linking contract to a farmer
        [Required]
        public int FarmerId { get; set; }

        // Navigation property for related farmer
        public Farmer? Farmer { get; set; }

        // Contract start date
        [Required]
        public DateTime StartDate { get; set; }

        // Contract end date
        [Required]
        public DateTime EndDate { get; set; }

        // Contract status used in workflow rules
        // Examples: Draft, Active, Expired, On Hold
        [Required]
        public string Status { get; set; }

        // Type of produce supplied by farmer
        // Examples: Maize, Fruit, Vegetables, Livestock
        [Required]
        public string ProduceType { get; set; }

        // Service package or agreement level
        [Required]
        public string ServiceLevel { get; set; }

        // Stores uploaded PDF agreement path
        public string? SignedAgreementPath { get; set; }

        // One contract can have many service requests
        public List<FarmServiceRequest> FarmServiceRequests { get; set; } = new();
    }
}