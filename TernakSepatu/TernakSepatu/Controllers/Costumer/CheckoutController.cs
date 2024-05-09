using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TernakSepatu.Controllers.Customer
{
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TernakSepatuDBContext _context;

        public CheckoutController(UserManager<ApplicationUser> userManager, TernakSepatuDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userAddresses = await _context.AddressInfos
                .Where(a => a.user_id == userId)
                .ToListAsync();

            var addressOptions = userAddresses.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.street}, {a.city}, {a.postal_code}"
            }).ToList();

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            decimal totalAmount = cartItems.Sum(c => c.Quantity * c.Price);

            var checkoutViewModel = new Checkout
            {
                CartItems = cartItems,
                TotalAmount = totalAmount,
                AddressOptions = addressOptions
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Checkout model)
        {
            var userId = await GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.CartItems == null)
            {
                // If CartItems is null, return a response with an error message
                return BadRequest("Cart is empty. Please add items to your cart before placing an order.");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            decimal totalAmount = cartItems.Sum(c => c.Quantity * c.Price);

            // Add $50 if the selected shipping method is JNT
            if (model.ShippingMethod == "JNT")
            {
                totalAmount += 50000;
            }

            if (model.ShippingMethod == "JNT" && model.PaymentMethod == "COD")
            {
                totalAmount -= 50000;
            }

            string status = model.PaymentMethod == "COD" ? "Menunggu Konfirmasi Admin" : "Menunggu Pembayaran";

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                PaymentMethod = model.PaymentMethod,
                ShippingMethod = model.ShippingMethod,
                ShippingAddress = GetSelectedAddress(model.SelectedAddressId),
                Status = status,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PricePerUnit = cartItem.Price
                };

                order.OrderDetails.Add(orderDetail);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear user's cart after placing order
            var userCartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            _context.Carts.RemoveRange(userCartItems);
            await _context.SaveChangesAsync();

            // Determine message based on payment method
            string message = "";
            if (model.PaymentMethod == "COD")
            {
                message = "Pesanan Anda sedang diproses.";
            }
            else
            {
                message = "Pesanan Anda telah dikirim ke admin, silahkan lakukan pembayaran.";
            }

            TempData["OrderMessage"] = message; // Use TempData to display message after redirect

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }


        private string GetSelectedAddress(int addressId)
        {
            var selectedAddress = _context.AddressInfos.FirstOrDefault(a => a.Id == addressId);

            if (selectedAddress != null)
            {
                return $"{selectedAddress.street}, {selectedAddress.city}, {selectedAddress.postal_code}, {selectedAddress.country}";
            }

            return null; // Return null jika alamat tidak ditemukan
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            // Cari pesanan berdasarkan orderId
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound(); // Tampilkan 404 jika pesanan tidak ditemukan
            }

            // Setel ViewBag.orderId dengan nilai orderId dari pesanan
            ViewBag.orderId = order.OrderId;

            // Tampilkan halaman OrderConfirmation dengan data pesanan
            return View(order);
        }



        private async Task<string> GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                // If using ASP.NET Core Identity
                var user = await _userManager.GetUserAsync(User);
                return user?.Id;
            }

            return null; // Return null if user is not authenticated
        }
    }
}
