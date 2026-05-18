namespace AgriConnectPrototype.Services
{
    // Handles business rules for farm service requests.
    // A request cannot be created if the contract is Expired or On Hold.

    public class ServiceRequestRules
    {
        // Checks whether a service request is allowed
        public bool CanCreateServiceRequest(string contractStatus)
        {
            // Return true only for valid contract statuses
            return contractStatus != "Expired" &&
                   contractStatus != "On Hold";
        }
    }
}