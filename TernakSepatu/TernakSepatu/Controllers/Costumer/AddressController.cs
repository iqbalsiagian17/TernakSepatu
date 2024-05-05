using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TernakSepatu.Data;
using TernakSepatu.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Add this namespace

namespace TernakSepatu.Controllers.Customer
{
    public class AddressController : Controller
    {
        private readonly TernakSepatuDBContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AddressController(TernakSepatuDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        // Menampilkan form untuk membuat alamat baru
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddressInfoDto addressInfoDto)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                AddressInfo addressinfo = new AddressInfo
                {
                    street = addressInfoDto.street,
                    city = addressInfoDto.city,
                    state = addressInfoDto.state,
                    country = addressInfoDto.country,
                    postal_code = addressInfoDto.postal_code,
                    user_id = currentUserId // Menetapkan ID pengguna saat ini
                };

                context.AddressInfos.Add(addressinfo);
                context.SaveChanges();

                return RedirectToAction("Index", "Address");
            }

            return View("create", addressInfoDto);
        }


        public IActionResult Index()
        {
            // Retrieve the current user's user_id
            string currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Filter addresses based on the current user's user_id
            var userAddresses = context.AddressInfos.Where(a => a.user_id == currentUserId).ToList();

            return View(userAddresses);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the address to be edited
            var address = context.AddressInfos.FirstOrDefault(a => a.Id == id);

            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,AddressInfoDto addressInfoDto)
        {
            var address = context.AddressInfos.Find(id);

            if (ModelState.IsValid)
            {
                address.street = addressInfoDto.street;
                address.city = addressInfoDto.city;
                address.state = addressInfoDto.state;
                address.country = addressInfoDto.country;
                address.postal_code = addressInfoDto.postal_code;

                context.SaveChanges();
            }
            return RedirectToAction("Index", "Address");

        }

        public IActionResult Delete(int id)
        {
            var address = context.AddressInfos.Find(id);
            if (address == null)
            {
                return RedirectToAction("Index", "Address");
            }

            context.AddressInfos.Remove(address); // Remove the entity from the context
            context.SaveChanges(); // Commit the deletion

            return RedirectToAction("Index", "Address");
        }


    }
}
