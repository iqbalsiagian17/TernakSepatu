using Microsoft.AspNetCore.Mvc;
using TernakSepatu.Data;
using TernakSepatu.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TernakSepatu.Controllers.Admin
{
    public class LandingPageController : Controller
    {
        private readonly TernakSepatuDBContext context;
        private readonly IWebHostEnvironment environment;

        public LandingPageController(TernakSepatuDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var landingpage = context.LandingPage.ToList();
            return View(landingpage);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(LandingPageDto landingpageDto)
        {
            if (landingpageDto.ImageURL == null)
            {
                ModelState.AddModelError("ImageURL", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(landingpageDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(landingpageDto.ImageURL!.FileName);

            string imageFullPath = environment.WebRootPath + "/Images/LandingPage/" + newFileName;
            using (var steam = System.IO.File.Create(imageFullPath))
            {
                landingpageDto.ImageURL.CopyTo(steam);
            }

            LandingPage landingpage = new LandingPage
            {
                Text = landingpageDto.Text,
                Status = landingpageDto.Status,
                ImageURL = newFileName,
                CreatedAt = DateTime.Now, // Mengisi properti CreateAt dengan nilai saat ini
            };

            context.LandingPage.Add(landingpage);
            context.SaveChanges();

            return RedirectToAction("Index", "LandingPage");
        }


        public IActionResult Edit(int id)
        {
            var landingpage = context.LandingPage.Find(id);

            if (landingpage == null)
            {
                return RedirectToAction("Index", "LandingPage");
            }

            var landingpageDto = new LandingPageDto
            {
                Text = landingpage.Text,
                Status = landingpage.Status,

            };

            ViewData["LandingPageId"] = landingpage.Id;
            ViewData["CreatedAt"] = landingpage.CreatedAt;
            ViewData["ImageURL"] = landingpage.ImageURL;


            return View(landingpageDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, LandingPageDto landingpageDto)
        {
            var landingpage = context.LandingPage.Find(id);

            if (landingpage == null)
            {
                return RedirectToAction("Index", "LandingPage");
            }

            if (ModelState.IsValid)
            {
                string newFileName = landingpage.ImageURL;
                if (landingpageDto.ImageURL != null)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(landingpageDto.ImageURL.FileName);

                    string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "LandingPage", newFileName);
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        landingpageDto.ImageURL.CopyTo(stream);
                    }

                    // Delete old image if it exists
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", "LandingPage", landingpage.ImageURL);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }

                // Update landingpage properties including ImageURL
                landingpage.Text = landingpageDto.Text;
                landingpage.Status = landingpageDto.Status;
                landingpage.ImageURL = newFileName;

                // Save changes to the database
                context.SaveChanges();

                return RedirectToAction("Index", "LandingPage");
            }

            // If ModelState is not valid, return the view with the provided landingpageDto
            ViewData["CreatedAt"] = landingpage.CreatedAt;
            ViewData["ImageURL"] = landingpage.ImageURL;
            return View(landingpageDto);
        }


        public IActionResult Delete(int id)
        {
            var landingpage = context.LandingPage.Find(id);
            if (landingpage == null)
            {
                return RedirectToAction("Index", "LandingPage");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", "LandingPage", landingpage.ImageURL);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            context.LandingPage.Remove(landingpage);
            context.SaveChanges(true);

            return RedirectToAction("Index", "LandingPage");
        }
    }
}
