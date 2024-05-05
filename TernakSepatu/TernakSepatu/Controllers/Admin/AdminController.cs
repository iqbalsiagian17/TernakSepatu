using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace TernakSepatu.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TernakSepatuDBContext _context;

        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, TernakSepatuDBContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context; // Menggunakan parameter 'context' untuk menginisialisasi field '_context'
        }



        public async Task<IActionResult> Dashboard()
        {
            // Dapatkan ID pengguna yang sedang masuk
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            // Periksa apakah pengguna memiliki peran tertentu (misalnya, Admin)
            if (roles.Contains("admin"))
            {
                // Retrieve counts of products, brands, and confirmed orders from your database
                var productCount = _context.Product.Count();
                var brandCount = _context.Brand.Count();
                var confirmedOrderCount = _context.Orders.Where(o => o.Status == "Confirmed").Count();

                // Pass the counts to the view
                ViewBag.ProductCount = productCount;
                ViewBag.BrandCount = brandCount;
                ViewBag.ConfirmedOrderCount = confirmedOrderCount;

                // Cek apakah ada bukti pembayaran yang statusnya "Pending"
                var pendingProofs = _context.PaymentQrProofs.Where(p => p.Status == "Pending").ToList();

                if (pendingProofs.Any())
                {
                    // Jika ada bukti pembayaran yang masih "Pending", buat notifikasi
                    ViewData["Notification"] = "Pelanggan menunggu konfirmasi bukti pembayarannya. Silahkan periksa.";
                    ViewData["PendingProofsCount"] = pendingProofs.Count;
                }

                return View();
            }
            else
            {
                // Jika tidak, kembalikan ke halaman lain atau tampilkan pesan akses ditolak
                return RedirectToAction("AccessDenied", "Admin"); // Contoh redirect ke halaman AccessDenied
            }
        }


    }
}


