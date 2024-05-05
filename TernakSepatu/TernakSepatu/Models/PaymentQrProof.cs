using System.ComponentModel.DataAnnotations;
using TernakSepatu.Areas.Identity.Data;


namespace TernakSepatu.Models
{
    public class PaymentQrProof
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProofImage { get; set; }
        public string Status { get; set; } // Status values: 'Pending', 'Accept

        // Navigation property for the associated order
        public Order Order { get; set; }


    }
}
