using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TernakSepatu.Views.Costumer
{
    [Authorize(Roles = "costumer")]

    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
