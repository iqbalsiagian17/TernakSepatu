using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TernakSepatu.Areas.Identity.Data;

namespace TernakSepatu.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property to link with ApplicationUser (AspNetUsers)
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }

    }

   
}

