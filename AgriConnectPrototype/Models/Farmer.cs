using System.ComponentModel.DataAnnotations;

namespace AgriConnectPrototype.Models
{
    // Stores farmer information in the Farmers Management System.
    // A farmer can have multiple supply contracts.

    public class Farmer
    {
        // Unique farmer ID
        public int FarmerId { get; set; }

        // Farmer full name
        [Required]
        public string FullName { get; set; }

        // Contact number used for communication
        [Required]
        public string ContactNumber { get; set; }

        // Farmer email address
        [Required]
        public string Email { get; set; }

        // Name of the farm
        [Required]
        public string FarmName { get; set; }

        // Farming region (e.g., Gauteng, Limpopo)
        [Required]
        public string Region { get; set; }

        // One farmer can have many supply contracts
        public List<SupplyContract> SupplyContracts { get; set; } = new();
    }
}