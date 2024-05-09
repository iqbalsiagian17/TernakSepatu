using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class PaymentQrMethodDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IFormFile? ImageUrl { get; set; }
    }
}
