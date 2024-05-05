using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class LandingPageDto
    {
        public int Id { get; set; }

        public IFormFile? ImageURL { get; set; }

        [Required]
        public string Text { get; set; }

        public string Status { get; set; }

    }
}
