using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Category ID")]
        public int CategoryID { get; set; }

        [Display(Name = "SubCategory ID")]
        public int SubCategoryID { get; set; }

        [Display(Name = "Brand ID")]
        public int BrandID { get; set; }

        public int Size { get; set; }


        public string Colors { get; set; }

        public string Details { get; set; }

        [Display(Name = "Image URL")]

        public List<IFormFile> ImageURL { get; set; } // Use List<IFormFile> to store multiple files

        public string Stock { get; set; } // Changed type to string

        public string Condition { get; set; } // Changed type to string

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public string Gender { get; set; }
    }
}
