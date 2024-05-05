using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Customer
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TernakSepatuDBContext _context; // Tambahkan referensi ke TernakSepatuDBContext
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CartController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, TernakSepatuDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            this._userManager = userManager;
            _context = context; // Inisialisasi _context
            _httpContextAccessor = httpContextAccessor;
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


        // Menampilkan isi keranjang berdasarkan UserId
        public async Task<IActionResult> Index()
        {
            var userId = await GetUserUniqueId();

            if (string.IsNullOrEmpty(userId))
            {
                // Handle case where user is not authenticated
                // For example, redirect to login page or display an error message
                return RedirectToAction("Login", "Account");
            }

            // Pass userId to the view using ViewData or a view model
            ViewData["UserId"] = userId;


            var cartItems = await _context.Carts
                .Include(c => c.Product) // Include the related Product entity
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }


        public async Task<IActionResult> AddToCart(int id)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Product not found
            }

            var userId = await GetUserUniqueId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Handle unauthorized access
            }

            // Check if the product already exists in the user's cart
            var existingCartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == id);
            if (existingCartItem != null)
            {
                return Json(new { success = false, message = "Product already exists in cart." });
            }
            else
            {
                // Create a new Cart entry
                var cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = id,
                    Quantity = 1, // Set the quantity to 1
                    Price = product.Price,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Add to database
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Return JSON response indicating success
            return Json(new { success = true, message = "Product added to cart." });
        }





        // Menghapus item dari keranjang
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);

            if (cartItem == null)
            {
                return NotFound(); // Cart item not found
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = cartItem.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found."); // Atau handle kasus ini sesuai kebutuhan
            }

            var userId = user.Id;

            // Mengambil jumlah item keranjang dari database berdasarkan userId
            var cartItemCount = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return Ok(cartItemCount); // Mengembalikan jumlah item sebagai respons OK (200)
        }




    }

}
