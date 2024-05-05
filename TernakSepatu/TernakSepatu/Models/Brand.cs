using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        public string ImageURL { get; set; }

    }
}
