using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TernakSepatu.Models
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "User ID")]
        public string UserID { get; set; }

        public decimal Rating { get; set; }

        public string Comment { get; set; }
    }
}
