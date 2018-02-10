using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using Cstieg.Sales.Models;
using Cstieg.Geography;
using Cstieg.Sales.Repositories;

namespace DeerflyPatches.Models
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

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, ISalesDbContext
    {
        public ApplicationDbContext()
            : base(ConfigurationManager.AppSettings["DbConnection"], throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(string connection) : base(connection)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<Store> Stores { get; set; }
        public IDbSet<ShoppingCart> ShoppingCarts { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderDetail> OrderDetails { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<PromoCode> PromoCodes { get; set; }
        public IDbSet<PromoCodeAdded> PromoCodesAdded { get; set; }
        public IDbSet<Retailer> Retailers { get; set; }
        public IDbSet<LatLng> LatLngs { get; set; }
        public IDbSet<Testimonial> Testimonials { get; set; }
        public IDbSet<WebImage> WebImages { get; set; }
        public IDbSet<Cstieg.Sales.Models.Country> Countries { get; set; }
        public IDbSet<ShippingCountry> ShippingCountries { get; set; }
        public IDbSet<ShippingScheme> ShippingSchemes { get; set; }

    }
}