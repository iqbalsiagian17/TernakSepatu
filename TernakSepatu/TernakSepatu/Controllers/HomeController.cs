using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TernakSepatuDBContext _context; // Tambahkan referensi ke TernakSepatuDBContext
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, TernakSepatuDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            this._userManager = userManager;
            _context = context; // Inisialisasi _context
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var landingPages = _context.LandingPage.ToList();
            ViewData["LandingPages"] = landingPages;

            // Ambil data dari tabel Brand
            var brands = _context.Brand.ToList();
            ViewData["Brands"] = brands;

            // Ambil data dari tabel Product
            var products = _context.Product.ToList();
            ViewData["Products"] = products;

            // Ambil data dari tabel Product
            var category = _context.Category.ToList();
            ViewData["Category"] = category;


            // Hitung jumlah total data dari tabel Produk
            var totalProducts = _context.Product.Count();
            ViewData["TotalProducts"] = totalProducts;

            ViewData["UserID"] = _userManager.GetUserId(this.User);

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                string currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userAddresses = _context.AddressInfos.FirstOrDefault(ai => ai.user_id == currentUserId);

                if (userAddresses == null)
                {
                    TempData["ShowAddAddressPopup"] = true;
                }
            }



            return View(_context); // Pass the database context to the view
        }



        public IActionResult Detail(int id)
        {
            // Ambil data dari tabel Product
            var category = _context.Category.ToList();
            ViewData["Category"] = category;

            // Ambil data produk dari database berdasarkan ID
            var product = _context.Product.FirstOrDefault(p => p.Id == id);

            var products = _context.Product.ToList();
            ViewData["Products"] = products;

            // Pastikan produk ditemukan
            if (product == null)
            {
                return NotFound(); // Kembalikan response 404 Not Found jika produk tidak ditemukan
            }
            var brand = _context.Brand.FirstOrDefault(b => b.Id == product.BrandID);
            ViewData["Brand"] = brand; // Kirim merek (brand) ke ViewData

            // Kirim data produk ke view
            return View(product);
        }



        public IActionResult Shop(string categoryName, string brandName)
        {
            var brands = _context.Brand.ToList();
            ViewData["Brands"] = brands;

            List<Product> filteredProducts;

            if (!string.IsNullOrEmpty(categoryName))
            {
                // Filter products based on the selected category
                filteredProducts = _context.Product
                                        .Where(p => p.Category.CategoryName == categoryName)
                                        .ToList();
                ViewData["CategoryName"] = categoryName; // Set the selected category name
            }
            else if (!string.IsNullOrEmpty(brandName))
            {
                // Filter products based on the selected brand
                filteredProducts = _context.Product
                                        .Where(p => p.Brand.BrandName == brandName)
                                        .ToList();
                ViewData["BrandName"] = brandName; // Set the selected brand name
            }
            else
            {
                // Display all products if no filter is applied
                filteredProducts = _context.Product.ToList();
            }

            // Get unique colors from the filtered products
            var colors = filteredProducts.Select(p => p.Colors).Distinct().OrderBy(c => c).ToList();
            ViewData["Colors"] = colors;

            var size = filteredProducts.Select(p => p.Size).Distinct().OrderBy(c => c).ToList();
            ViewData["Size"] = size;

            var conditon = filteredProducts.Select(p => p.Condition).Distinct().OrderBy(c => c).ToList();
            ViewData["Condition"] = conditon;

            ViewData["Products"] = filteredProducts;

            var categories = _context.Category.ToList();
            ViewData["Category"] = categories;

            return View();
        }


        [Authorize]

        public IActionResult AddToCart(int id)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = GetUserUniqueId().Result;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Handle unauthorized access
            }

            List<int> cart = GetCartFromSession(userId);
            cart.Add(id);
            SaveCartToSession(cart, userId);

            return RedirectToAction("Index");
        }
        [Authorize]

        public IActionResult ViewCart()
        {
            var userId = GetUserUniqueId().Result;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Handle unauthorized access
            }

            List<int> cart = GetCartFromSession(userId);
            List<Product> cartProducts = new List<Product>();

            foreach (var productId in cart)
            {
                var product = _context.Product.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    cartProducts.Add(product);
                }
            }

            return View(cartProducts);
        }

        public IActionResult RemoveFromCart(int id)
        {
            var userId = GetUserUniqueId().Result;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Handle unauthorized access
            }

            List<int> cart = GetCartFromSession(userId);

            if (cart.Contains(id))
            {
                cart.Remove(id);
                SaveCartToSession(cart, userId);
            }

            return RedirectToAction("ViewCart");
        }

        private async Task<string> GetUserUniqueId()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                return user.Id;
            }
            return null;
        }


        private List<int> GetCartFromSession(string userId)
        {
            var sessionKey = "Cart_" + userId;
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(sessionKey);

            return string.IsNullOrEmpty(cartJson) ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(cartJson);
        }


        private void SaveCartToSession(List<int> cart, string userId)
        {
            var sessionKey = "Cart_" + userId;
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonConvert.SerializeObject(cart);

            session.SetString(sessionKey, cartJson);
        }



        public IActionResult GetCartItemCount()
        {
            var userId = GetUserUniqueId().Result;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Handle unauthorized access
            }

            List<int> cart = GetCartFromSession(userId);
            int itemCount = cart.Count;

            return Json(itemCount);
        }








    }
}
