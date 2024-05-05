using TernakSepatu.Areas.Identity.Data;

namespace TernakSepatu.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }

        public string Status { get; set; }
        public ApplicationUser User { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public string ShippingAddress { get; set; } // Properti alamat tujuan (shipping address)

    }
}
