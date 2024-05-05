using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers
{
    public class OrderController : Controller
    {
        private readonly TernakSepatuDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(TernakSepatuDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string> GetUserUniqueId()
        {
            if (User.Identity.IsAuthenticated)
            {
                // If using ASP.NET Core Identity
                var user = await _userManager.GetUserAsync(User);
                return user?.Id;
            }

            return null; // Return null if user is not authenticated
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserUniqueId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Ambil semua pesanan milik pengguna saat ini
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderDetails) // Sertakan detail pesanan
                .ToListAsync();

            return View(userOrders);
        }

        public async Task<IActionResult> Detail(int OrderId)
        {
            var userId = await GetUserUniqueId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Ambil pesanan berdasarkan ID dan pastikan pesanan tersebut dimiliki oleh pengguna yang sedang masuk
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) // Sertakan informasi Product
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);

            if (order == null)
            {
                return NotFound(); // Pesanan tidak ditemukan atau tidak dimiliki oleh pengguna yang sedang masuk
            }

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmDelivery(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || !order.Status.StartsWith("Pesanan Telah dikirim"))
            {
                return NotFound(); // Handle invalid order or status
            }

            // Update order status to "Paket telah diterima"
            order.Status = "Paket telah diterima";
            await _context.SaveChangesAsync();

            // Mengubah status stok produk terkait dari "Ready" menjadi "Habis"
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = orderDetail.Product;
                if (product != null && product.Stock == "Ready")
                {
                    // Mengubah status stok menjadi "Habis"
                    product.Stock = "Habis";
                    // Simpan perubahan ke database
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }



    }
}
