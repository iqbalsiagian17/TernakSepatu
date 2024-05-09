using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Admin
{
    public class ConfirmationAdminController : Controller
    {
        private readonly TernakSepatuDBContext _context;

        public ConfirmationAdminController(TernakSepatuDBContext context)
        {
            _context = context;
        }

        // Action untuk menampilkan daftar orders
        public IActionResult Index()
        {
            try
            {
                var orders = _context.Orders.ToList();
                return View(orders);
            }
            catch (Exception ex)
            {
                // Tangani pengecualian di sini
                Console.WriteLine($"An error occurred while fetching orders: {ex.Message}");
                throw; // Re-throw the exception to let it bubble up
            }
        }


        // Action untuk mengubah status order dari "Menunggu Konfirmasi Admin" menjadi "Pesanan Anda sedang diproses"
        public IActionResult ProcessOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null || order.Status != "Menunggu Konfirmasi Admin")
            {
                return NotFound(); // Order tidak ditemukan atau status bukan "Menunggu Konfirmasi Admin"
            }

            order.Status = "Pesanan Anda sedang diproses";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Action untuk mengubah status order dari "Pesanan Anda sedang diproses" menjadi "Pesanan Telah dikirim"
        [HttpGet]
        public IActionResult ShipOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pesanan Anda sedang diproses")
            {
                return NotFound(); // Order tidak ditemukan atau status bukan "Pesanan Anda sedang diproses"
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult ShipOrder(int orderId, string trackingNumber)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pesanan Anda sedang diproses")
            {
                return NotFound(); // Order tidak ditemukan atau status bukan "Pesanan Anda sedang diproses"
            }

            // Update status order menjadi "Pesanan Telah dikirim" dan simpan nomor resi
            order.Status = $"Pesanan Telah dikirim (Nomor Resi: {trackingNumber})";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
