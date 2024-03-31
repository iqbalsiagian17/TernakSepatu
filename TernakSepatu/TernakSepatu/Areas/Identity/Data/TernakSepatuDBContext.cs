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
