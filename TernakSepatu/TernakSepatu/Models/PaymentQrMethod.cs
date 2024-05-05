using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class PaymentQrMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
