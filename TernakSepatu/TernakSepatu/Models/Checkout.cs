using Microsoft.AspNetCore.Mvc.Rendering;

namespace TernakSepatu.Models
{
    public class Checkout
    {
        public List<Cart> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public AddressInfo UserAddress { get; set; }
        public string ShippingMethod { get; set; } // Untuk menyimpan pilihan shipping method dari form
        public string PaymentMethod { get; set; } // Untuk menyimpan pilihan payment method dari form

        public List<SelectListItem> AddressOptions { get; set; }
        public int SelectedAddressId { get; set; } // Properti untuk menyimpan ID alamat yang dipilih

    }
}
