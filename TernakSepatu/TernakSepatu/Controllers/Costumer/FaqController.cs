using Microsoft.AspNetCore.Mvc;

namespace TernakSepatu.Controllers.Costumer
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
