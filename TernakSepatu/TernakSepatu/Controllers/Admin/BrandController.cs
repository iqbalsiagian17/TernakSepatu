using Microsoft.AspNetCore.Mvc;
using System;
using TernakSepatu.Data;
using TernakSepatu.Dtos;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Admin
{
    public class BrandController : Controller
    {
        private readonly TernakSepatuDBContext context;
        private readonly IWebHostEnvironment environment;

        public BrandController(TernakSepatuDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;

        }

        public IActionResult Index()
        {
            var brands = context.Brand.ToList();
            return View(brands);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BrandDto brandDto)
        {
            if (brandDto.ImageURL == null)
            {
                ModelState.AddModelError("ImageURL", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(brandDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(brandDto .ImageURL!.FileName);

            string imageFullPath = environment.WebRootPath + "/Images/Brand/" + newFileName;
            using (var steam = System.IO.File.Create(imageFullPath))
            {
                brandDto.ImageURL.CopyTo(steam);
            }

            Brand brand = new Brand
            {
                BrandName = brandDto.BrandName,
                ImageURL = newFileName,
            };

            context.Brand.Add(brand);
            context.SaveChanges();

            return RedirectToAction("Index", "Brand");
        }

        public IActionResult Edit(int id)
        {
            var brand = context.Brand.Find(id);

            if (brand == null)
            {
                return RedirectToAction("Index", "Brand");
            }

            var brandDto = new BrandDto
            {
                BrandName = brand.BrandName,

            };

            ViewData["ImageURL"] = brand.ImageURL;


            return View(brandDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, BrandDto brandDto)
        {
            var brand = context.Brand.Find(id);

            if (brand == null)
            {
                return RedirectToAction("Index", "Brand");
            }

            if (ModelState.IsValid)
            {
                string newFileName = brand.ImageURL;
                if (brandDto.ImageURL != null)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(brandDto.ImageURL.FileName);

                    string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "Brand", newFileName);
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        brandDto.ImageURL.CopyTo(stream);
                    }

                    // Delete old image if it exists
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", "Brand", brand.ImageURL);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }

                // Update landingpage properties including ImageURL
                brand.BrandName = brandDto.BrandName;
                brand.ImageURL = newFileName;

                // Save changes to the database
                context.SaveChanges();

                return RedirectToAction("Index", "Brand");
            }

            // If ModelState is not valid, return the view with the provided landingpageDto
            ViewData["ImageURL"] = brand.ImageURL;
            return View(brandDto);
        }

        public IActionResult Delete(int id)
        {
            var brand = context.Brand.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index", "Brand");
            }

            // Remove related products
            var products = context.Product.Where(p => p.BrandID == id).ToList();
            foreach (var product in products)
            {
                context.Product.Remove(product);
            }

            // Delete brand
            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "Brand", brand.ImageURL);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }
            context.Brand.Remove(brand);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Brand");
        }



    }
}
