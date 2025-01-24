using System.ComponentModel.DataAnnotations;

namespace TallyIntegrationAPI.Models
{
    public class LedgerRequest
    {
        [Required(ErrorMessage = "Ledger name is required.")]
        [MaxLength(100, ErrorMessage = "Ledger name cannot exceed 100 characters.")]
        public string LedgerName { get; set; }

        [Required(ErrorMessage = "Parent group is required.")]
        [MaxLength(50, ErrorMessage = "Parent group cannot exceed 50 characters.")]
        public string ParentGroup { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public bool IsUpdate { get; set; }
    }
}
