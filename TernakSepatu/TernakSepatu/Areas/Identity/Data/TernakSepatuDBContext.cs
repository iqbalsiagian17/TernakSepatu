using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TernakSepatu.Areas.Identity.Data;
using TernakSepatu.Models;

namespace TernakSepatu.Data;

public class TernakSepatuDBContext : IdentityDbContext<ApplicationUser>
{
    public TernakSepatuDBContext(DbContextOptions<TernakSepatuDBContext> options)
        : base(options)
    {
    }

    public DbSet<LandingPage> LandingPage {  get; set; }

    public DbSet<AddressInfo> AddressInfos { get; set; }
    public DbSet<Brand> Brand { get; set; }

    public DbSet<Category> Category { get; set; }

    public DbSet<SubCategory> SubCategory { get; set; }

    public DbSet<Product> Product { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<Wishlist> Wishlists { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<PaymentQrProof> PaymentQrProofs { get; set; }
    public DbSet<PaymentQrMethod> PaymentQrMethods { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var admin = new IdentityRole("admin");
        admin.NormalizedName = "admin";

        var costumer = new IdentityRole("costumer");
        costumer.NormalizedName = "client";

        builder.Entity<IdentityRole>().HasData(admin, costumer);
    }
}
