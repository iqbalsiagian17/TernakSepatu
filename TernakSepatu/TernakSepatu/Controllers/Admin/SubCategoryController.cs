using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TernakSepatu.Data;
using TernakSepatu.Dtos;
using TernakSepatu.Models;
using System.Linq;

namespace TernakSepatu.Controllers.Admin
{
    public class SubCategoryController : Controller
    {
        private readonly TernakSepatuDBContext context;

        public SubCategoryController(TernakSepatuDBContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var categories = context.Category.ToList(); // Fetch categories from the database
            ViewBag.Categories = categories; // Assign categories to ViewBag


            var subcategories = context.SubCategory.Include(s => s.Category).ToList(); // Fetch subcategories with their related category and brand
            return View(subcategories);
        }

        public IActionResult Create()
        {
            var categories = context.Category.ToList(); // Fetch categories from the database
            ViewBag.Categories = categories; // Assign categories to ViewBag

            return View();
        }

        [HttpPost]
        public IActionResult Create(SubCategoryDto subcategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(subcategoryDto);
            }

            // Check if the provided CategoryID is valid
            var category = context.Category.Find(subcategoryDto.CategoryID);
            if (category == null)
            {
                // Return the view with an error message if CategoryID is invalid
                ModelState.AddModelError("CategoryID", "Invalid CategoryID.");
                return View(subcategoryDto);
            }


            // If CategoryID is valid, create a new SubCategory
            SubCategory subcategory = new SubCategory
            {
                SubCategoryName = subcategoryDto.SubCategoryName,
                CategoryID = subcategoryDto.CategoryID,
                Category = category  // Set the relationship with the valid Category
            };

            context.SubCategory.Add(subcategory);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var subcategory = context.SubCategory.Include(s => s.Category).FirstOrDefault(s => s.Id == id);
            if (subcategory == null)
            {
                return RedirectToAction("Index");
            }

            var categories = context.Category.ToList(); // Fetch categories from the database
            ViewBag.Categories = categories; // Assign categories to ViewBag

            
            var subcategoryDto = new SubCategoryDto
            {
                SubCategoryName = subcategory.SubCategoryName,
                CategoryID = subcategory.CategoryID,

            };

            return View(subcategoryDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, SubCategoryDto subcategoryDto)
        {
            var subcategory = context.SubCategory.Find(id);
            if (subcategory == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(subcategoryDto);
            }

            var category = context.Category.Find(subcategoryDto.CategoryID);
            if (category == null)
            {
                ModelState.AddModelError("CategoryID", "Invalid CategoryID.");
                return View(subcategoryDto);
            }


            subcategory.SubCategoryName = subcategoryDto.SubCategoryName;
            subcategory.CategoryID = subcategoryDto.CategoryID;
            subcategory.Category = category;


            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var subcategory = context.SubCategory.Find(id);
            if (subcategory == null)
            {
                return RedirectToAction("Index");
            }

            context.SubCategory.Remove(subcategory);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
