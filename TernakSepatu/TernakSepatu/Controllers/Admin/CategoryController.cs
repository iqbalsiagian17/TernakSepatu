using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using TernakSepatu.Data;
using TernakSepatu.Dtos;
using TernakSepatu.Models;

namespace TernakSepatu.Controllers.Admin
{
    public class CategoryController : Controller 
    {
        private readonly TernakSepatuDBContext context;

        public CategoryController(TernakSepatuDBContext context)
        {
            this.context = context;

        }

        public IActionResult Index()
        {
            var category = context.Category.ToList();
            return View(category);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }


            Category category = new Category
            {
                CategoryName = categoryDto.CategoryName,
            };

            context.Category.Add(category);
            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        public IActionResult Edit(int id)
        {
            var category = context.Category.Find(id);

            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            var categoryDto = new CategoryDto
            {
                CategoryName = category.CategoryName,

            };

            return View(categoryDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, CategoryDto categoryDto)
        {
            var category = context.Category.Find(id);

            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            if (ModelState.IsValid)
            {
                category.CategoryName = categoryDto.CategoryName;

                // Save changes to the database
                context.SaveChanges();

                return RedirectToAction("Index", "Category");
            }

            return View(categoryDto);
        }

        public IActionResult Delete(int id)
        {
            var category = context.Category.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            context.Category.Remove(category);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Category");
        }
    }
}
