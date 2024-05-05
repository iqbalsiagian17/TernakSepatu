using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers
{
    [Authorize] // Hanya pengguna yang terautentikasi yang dapat mengakses controller ini
    public class WishlistController : Controller
    {
        private readonly TernakSepatuDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(TernakSepatuDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Wishlist
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userWishlist = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Product) // Include the related Product entity
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(userWishlist);
        }

        // POST: /Wishlist/AddToWishlist
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int id)
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

            // Check if the product already exists in the user's wishlist
            var existingWishlistItem = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == id);
            if (existingWishlistItem != null)
            {
                return Json(new { success = false, message = "Product already exists in wishlist." });
            }

            // Create a new Wishlist entry
            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = id,
                CreatedAt = DateTime.Now
            };

            // Add to database
            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            // Return success without any specific message
            return Json(new { success = true });
        }



        // POST: /Wishlist/RemoveFromWishlist
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var wishlistItem = await _context.Wishlists.FindAsync(wishlistId);

            if (wishlistItem == null)
            {
                return NotFound(); // Wishlist item not found
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            // Ambil UserId dari wishlistItem untuk digunakan dalam redirect
            var userId = wishlistItem.UserId;

            // Redirect pengguna ke halaman wishlist setelah berhasil menghapus item
            return RedirectToAction("Index", new { userId });
        }


        private async Task<string> GetUserUniqueId()
        {
            if (User.Identity.IsAuthenticated)
            {
                // If using ASP.NET Core Identity, get the unique identifier (UserId) of the current user
                var user = await _userManager.GetUserAsync(User);
                return user?.Id;
            }

            return null; // Return null if user is not authenticated
        }

        [HttpGet]


        public async Task<IActionResult> GetWishlistItemCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var userId = user.Id;

            // Mengambil jumlah item wishlist dari database berdasarkan userId
            var wishlistItemCount = await _context.Wishlists
                .CountAsync(w => w.UserId == userId);

            return Ok(wishlistItemCount); // Mengembalikan jumlah item sebagai respons OK (200)
        }


    }
}
