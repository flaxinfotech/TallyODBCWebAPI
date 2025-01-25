using System.ComponentModel.DataAnnotations;

namespace TallyIntegrationAPI.Models
{
    public class LedgerRequest
    {
        public string LedgerName { get; set; }
        public string ParentGroup { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string Pincode { get; set; }
        public string GSTRegistrationType { get; set; } // Example: Regular, Composition, Unregistered
        public string GSTIN { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
    }

}
