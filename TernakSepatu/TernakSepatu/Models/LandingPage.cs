using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class LandingPage
    {
        internal object CreateAt;

        public int Id { get; set; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }

        [Required]
        public string Text { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
