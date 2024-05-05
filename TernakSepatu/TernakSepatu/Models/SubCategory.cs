using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TernakSepatu.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Display(Name = "SubCategory Name")]
        public string SubCategoryName { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }



        public Category Category { get; set; }
    }
}
