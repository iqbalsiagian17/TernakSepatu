using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TernakSepatu.Areas.Identity.Data;


namespace TernakSepatu.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; }
        public ApplicationUser User { get; set; }
    }
}
