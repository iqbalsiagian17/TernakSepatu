using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class LandingPage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
