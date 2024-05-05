using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using TernakSepatu.Data;
using TernakSepatu.Models;


namespace TernakSepatu.Controllers.Admin
{
    public class ProductController : Controller
    {
        private readonly TernakSepatuDBContext context;
        private readonly IWebHostEnvironment environment;

        public ProductController(TernakSepatuDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            // Fetch categories, subcategories, and brands from the database
            var categories = context.Category.ToList();
            var subcategories = context.SubCategory.ToList();
            var brands = context.Brand.ToList();

            // Assign categories, subcategories, and brands to ViewBag
            ViewBag.Categories = categories;
            ViewBag.SubCategories = subcategories;
            ViewBag.Brands = brands;

            // Fetch products from the database including related subcategory and brand
            var product = context.Product.ToList();

            return View(product);
        }


        public IActionResult Create()
        {
            // Fetch categories, subcategories, and brands from the database
            var categories = context.Category.ToList();
            var subcategories = context.SubCategory.ToList();
            var brands = context.Brand.ToList();

            // Assign categories, subcategories, and brands to ViewBag
            ViewBag.Categories = categories;
            ViewBag.SubCategories = subcategories;
            ViewBag.Brands = brands;

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageURL == null || productDto.ImageURL.Count == 0)
            {
                ModelState.AddModelError("ImageURL", "At least one image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            try
            {
                List<string> newFileNames = new List<string>();
                foreach (var imageFile in productDto.ImageURL)
                {
                    // Save the image file
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(imageFile.FileName);
                    string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "Product", newFileName);

                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    newFileNames.Add(newFileName);
                }

                    // Check if the provided IDs are valid
                    var category = context.Category.Find(productDto.CategoryID);
                var subcategory = context.SubCategory.Find(productDto.SubCategoryID);
                var brand = context.Brand.Find(productDto.BrandID);
                if (category == null || subcategory == null || brand == null)
                {
                    // Return the view with an error message if any ID is invalid
                    ModelState.AddModelError("", "One or more IDs are invalid.");
                    return View(productDto);
                }

                string imagesString = string.Join(",", newFileNames);


                // Create a new Product object
                Product product = new Product
                {
                    ProductName = productDto.ProductName,
                    CategoryID = productDto.CategoryID,
                    SubCategoryID = productDto.SubCategoryID,
                    BrandID = productDto.BrandID,
                    Size = productDto.Size,
                    Colors = productDto.Colors,
                    Details = productDto.Details,
                    ImageURL = imagesString, // Save concatenated file names
                    Price = productDto.Price,
                    Discount = productDto.Discount,
                    Gender = productDto.Gender,
                    Stock = productDto.Stock,
                    Condition = productDto.Condition,


                };

                // Save the product to the database
                context.Product.Add(product);
                context.SaveChanges();

                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ModelState.AddModelError("", "An error occurred while creating the product.");
                return View(productDto);
            }
        }
        // Method untuk menampilkan form edit dengan data yang telah ada
        public IActionResult Edit(int id)
        {
            var product = context.Product.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Fetch categories, subcategories, and brands from the database
            var categories = context.Category.ToList();
            var subcategories = context.SubCategory.ToList();
            var brands = context.Brand.ToList();

            // Assign categories, subcategories, and brands to ViewBag
            ViewBag.Categories = categories;
            ViewBag.SubCategories = subcategories;
            ViewBag.Brands = brands;

            return View(product);
        }

        // Method untuk mengubah data produk setelah melakukan edit
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Product.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            if (productDto.ImageURL != null && productDto.ImageURL.Count > 0)
            {
                // Jika input file untuk gambar baru diisi, lakukan penggantian gambar
                try
                {
                    List<string> newFileNames = new List<string>();
                    foreach (var imageFile in productDto.ImageURL)
                    {
                        // Save the image file
                        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(imageFile.FileName);
                        string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "Product", newFileName);

                        using (var stream = new FileStream(imageFullPath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        newFileNames.Add(newFileName);
                    }

                    // Hapus gambar lama
                    var oldImageNames = product.ImageURL.Split(',');
                    foreach (var oldImageName in oldImageNames)
                    {
                        var oldImagePath = Path.Combine(environment.WebRootPath, "Images", "Product", oldImageName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Simpan nama file yang baru diubah
                    string imagesString = string.Join(",", newFileNames);
                    product.ImageURL = imagesString;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while uploading and replacing the images.");
                    // Handle exception
                    return View(product); // Pass the existing product to the view
                }
            }

            // Update informasi produk yang tidak terkait dengan gambar
            product.ProductName = productDto.ProductName;
            product.CategoryID = productDto.CategoryID;
            product.SubCategoryID = productDto.SubCategoryID;
            product.BrandID = productDto.BrandID;
            product.Size = productDto.Size;
            product.Colors = productDto.Colors;
            product.Details = productDto.Details;
            product.Price = productDto.Price;
            product.Discount = productDto.Discount;
            product.Gender = productDto.Gender;
            product.Stock = productDto.Stock;
            product.Condition = productDto.Condition;

            try
            {
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while editing the product information.");
                // Handle exception
                return View(product); // Pass the existing product to the view
            }
        }


        public IActionResult Delete(int id)
        {
            var product = context.Product.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "Product", product.ImageURL);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            context.Product.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Product");
        }





        [HttpPost]
        public IActionResult DeleteImage(int productId, int index)
        {
            var product = context.Product.Find(productId);
            if (product == null)
            {
                return Json(new { success = false });
            }

            if (product.ImageURL == null)
            {
                return Json(new { success = false });
            }

            // Pisahkan daftar URL gambar
            var imageUrls = product.ImageURL.Split(',');

            // Pastikan index valid
            if (index >= 0 && index < imageUrls.Length)
            {
                // Hapus URL gambar dengan index yang diberikan
                var newImageUrls = imageUrls.Where((url, i) => i != index).ToList();
                product.ImageURL = string.Join(",", newImageUrls);

                // Simpan perubahan ke basis data
                context.SaveChanges();

                // Hapus file gambar dari sistem file
                var imageNameToDelete = imageUrls[index];
                var imagePath = Path.Combine(environment.WebRootPath, "Images", "Product", imageNameToDelete);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Jika berhasil, kembalikan status OK
                return Json(new { success = true });
            }

            // Jika terjadi kesalahan, kembalikan status tidak berhasil
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult ReplaceImage(int productId, int index, IFormFile image)
        {
            var product = context.Product.Find(productId);
            if (product == null)
            {
                return Json(new { success = false });
            }

            // Menghapus gambar lama
            var oldImageNames = product.ImageURL.Split(',');
            var imagePath = Path.Combine(environment.WebRootPath, "Images", "Product", oldImageNames[index]);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Menyimpan gambar baru
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(image.FileName);
            string newImageFullPath = Path.Combine(environment.WebRootPath, "Images", "Product", newFileName);

            using (var stream = new FileStream(newImageFullPath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            // Memperbarui nama file gambar di database
            oldImageNames[index] = newFileName;
            product.ImageURL = string.Join(",", oldImageNames);
            context.SaveChanges();

            return Json(new { success = true });
        }

        




    }
}
