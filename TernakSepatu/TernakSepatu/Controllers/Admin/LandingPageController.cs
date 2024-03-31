using Microsoft.AspNetCore.Mvc;
using TernakSepatu.Data;

namespace TernakSepatu.Controllers.Admin
{
    public class LandingPageController : Controller
    {
        private readonly TernakSepatuDBContext context;
        public LandingPageController(TernakSepatuDBContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var landingpage = context.LandingPage.ToList();
            return View(landingpage);
        }
    }
}
