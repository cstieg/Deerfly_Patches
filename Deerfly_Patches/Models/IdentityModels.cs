﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Deerfly_Patches.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Deerfly_Patches.Models.Address> Addresses { get; set; }
        public DbSet<Deerfly_Patches.Models.Customer> Customers { get; set; }
        public DbSet<Deerfly_Patches.Models.Order> Orders { get; set; }
        public DbSet<Deerfly_Patches.Models.OrderDetail> OrderDetails { get; set; }
        public DbSet<Deerfly_Patches.Models.Product> Products { get; set; }
        public DbSet<Deerfly_Patches.Models.PromoCode> PromoCodes { get; set; }

        public System.Data.Entity.DbSet<Deerfly_Patches.Models.Retailer> Retailers { get; set; }

        public System.Data.Entity.DbSet<Deerfly_Patches.Models.LatLng> LatLngs { get; set; }
    }
}