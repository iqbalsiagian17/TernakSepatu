namespace TernakSepatu.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }

        // Properti navigasi untuk relasi dengan Product
        public Product Product { get; set; }
        public Order Order { get; set; }

    }
}
