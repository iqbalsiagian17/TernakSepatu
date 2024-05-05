using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TernakSepatu.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        [ForeignKey("SubCategory")]
        public int SubCategoryID { get; set; }
        public SubCategory SubCategory { get; set; }

        [ForeignKey("Brand")]
        public int BrandID { get; set; }
        public Brand Brand { get; set; }

        public int Size { get; set; }

        public string Colors { get; set; }

        public string Details { get; set; }

        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public string Gender { get; set; }

        public string Condition { get; set; }

        public string Stock { get; set; } // Changed type to string

    }

}
