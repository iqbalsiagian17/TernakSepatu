using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Costumer
{
    public class QrPaymentController : Controller
    {
        private readonly TernakSepatuDBContext _context;
    
      public QrPaymentController(TernakSepatuDBContext context)
    {
        _context = context;
    }


        public IActionResult Index(int orderId)
        {
            // Get order details based on orderId
            Order order = GetOrderById(orderId);

            if (order == null)
            {
                return NotFound(); // Order not found, return 404
            }

            // Set ViewBag properties
            ViewBag.OrderId = order.OrderId;
            ViewBag.TotalAmount = order.TotalAmount; // Assuming TotalAmount is a decimal or double property

            var PaymentQrMethod = _context.PaymentQrMethods.ToList();
            ViewData["PaymentQrMethods"] = PaymentQrMethod;

            // Pass the order object to the view
            return View(order);
        }

        private Order GetOrderById(int orderId)
        {
            // Assuming _context is your database context containing orders
            Order order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            return order;
        }

        [HttpPost]
        public async Task<IActionResult> UploadProof(int orderId, IFormFile proofImage)
        {
            if (proofImage != null && proofImage.Length > 0)
            {
                var proofFileName = $"{orderId}_{Path.GetFileName(proofImage.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Proofs", proofFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await proofImage.CopyToAsync(stream);
                }

                // Save proof information to database
                var proof = new PaymentQrProof
                {
                    OrderId = orderId,
                    ProofImage = proofFileName,
                    Status = "Pending"
                };

                _context.PaymentQrProofs.Add(proof);
                await _context.SaveChangesAsync();

                // Set notification message
                TempData["Notification"] = "Bukti pembayaran berhasil dikirim, Silahkan Menunggu Konfirmasi Admin Di halaman Ini. Jika dirasa Terlalu lama anda dapat menghubungi 081240417200 / Admin Ternak Sepatu";

                // Redirect to Index action of Order controller
                return RedirectToAction("Index", "Order");
            }

            // If no file uploaded, return error
            return BadRequest();
        }


    }
}
