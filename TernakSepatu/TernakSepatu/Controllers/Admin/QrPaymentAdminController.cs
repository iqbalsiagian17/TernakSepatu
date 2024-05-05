using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Admin
{
    public class QrPaymentAdminController : Controller
    {
        private readonly TernakSepatuDBContext _context;
        private readonly IWebHostEnvironment environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public QrPaymentAdminController(TernakSepatuDBContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.environment = environment;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            var qr = _context.PaymentQrMethods.ToList();
            return View(qr);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        public IActionResult Create(PaymentQrMethodDto paymentqrmethodDto)
        {
            if (paymentqrmethodDto.ImageUrl == null)
            {
                ModelState.AddModelError("ImageUrl", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(paymentqrmethodDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(paymentqrmethodDto.ImageUrl!.FileName);

            string imageFullPath = environment.WebRootPath + "/Images/QrPayment/" + newFileName;
            using (var steam = System.IO.File.Create(imageFullPath))
            {
                paymentqrmethodDto.ImageUrl.CopyTo(steam);
            }

            PaymentQrMethod paymentqrmethod = new PaymentQrMethod
            {
                Name = paymentqrmethodDto.Name,
                ImageUrl = newFileName,
            };

            _context.PaymentQrMethods.Add(paymentqrmethod);
            _context.SaveChanges();

            return RedirectToAction("Index", "QrPaymentAdmin");
        }



        public IActionResult Approve()
        {
            var pendingProofs = _context.PaymentQrProofs
                .Include(p => p.Order) // Include navigation property 'Order'
                .ToList();

            return View(pendingProofs);
        }

        [HttpPost]
        public IActionResult ApproveProof(int proofId)
        {
            var proof = _context.PaymentQrProofs.FirstOrDefault(p => p.Id == proofId);

            if (proof == null)
            {
                return NotFound(); // Jika bukti pembayaran tidak ditemukan
            }

            // Mengubah status bukti pembayaran menjadi "Approved"
            proof.Status = "Approved";
            _context.SaveChanges();

            // Mengubah status pesanan terkait menjadi "Pesanan Anda sedang diproses"
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == proof.OrderId);

            if (order != null)
            {
                order.Status = "Pesanan Anda sedang diproses";
                _context.SaveChanges();
            }

            // Redirect kembali ke halaman Index untuk menampilkan daftar bukti pembayaran
            return RedirectToAction("Index");
        }

    }
}
