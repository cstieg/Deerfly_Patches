using System.Web.Mvc;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.Geography;
using Deerfly_Patches.Modules.PayPal;
using Deerfly_Patches.Models;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            return View(shoppingCart);
        }

        [HttpPost]
        public JsonResult VerifyAndSave()
        {
            string paymentDetailsJson = Request.Params.Get("paymentDetails");
            PaymentDetails paymentDetails = JsonConvert.DeserializeObject<PaymentDetails>(paymentDetailsJson);
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // get address and add to shopping cart
            AddressBase shippingAddress = paymentDetails.Payer.PayerInfo.ShippingAddress;

            if ((shoppingCart.Order.ShipToAddress.Country == "US" && shippingAddress.Country != "US") ||
                (shoppingCart.Order.ShipToAddress.Country != "US" && shippingAddress.Country == "US"))
            {
                // change to JSON error
                throw new ArgumentException("Your country does not match the country selected!");
            }

            shippingAddress.CopyTo(shoppingCart.Order.ShipToAddress);




            // verify items
            List<Item> items = (List<Item>) paymentDetails.Transactions.First().ItemList.Items;
            for (int i = 0; i < items.Count(); i++)
            {
                if (!shoppingCart.Order.OrderDetails.Exists(o => o.Product.ProductId == int.Parse(items[i].Sku) &&
                    o.Quantity == items[i].Quantity &&
                    o.UnitPrice == items[i].Price))
                {
                    throw new ArgumentException("Your shopping cart has been changed!  Please try again.");
                }
            }

            if (paymentDetails.Transactions.First().Amount.Total != shoppingCart.GrandTotal)
            {
                throw new ArgumentException("Your shopping cart has been changed! Please try again.");
            }


            // SAVE SHOPPING CART
            // update customer
            PayerInfo payerInfo = paymentDetails.Payer.PayerInfo;
            Customer customer = db.Customers.SingleOrDefault(c => c.EmailAddress == payerInfo.Email);
            bool isNewCustomer = customer == null;
            if (isNewCustomer)
            {
                customer = new Customer()
                {
                    Registered = DateTime.Now,
                    EmailAddress = payerInfo.Email
                };
            }
            customer.CustomerName = payerInfo.FirstName + " " +
                                    payerInfo.MiddleName + " " +
                                    payerInfo.LastName;
            customer.LastVisited = DateTime.Now;
            if (isNewCustomer)
            {
                db.Customers.Add(customer);
            }
            else
            {
                db.Entry(customer).State = EntityState.Modified;
            }

            // update address
            bool isNewAddress = true;
            if (!isNewCustomer)
            {
                AddressBase newAddress = paymentDetails.Payer.PayerInfo.ShippingAddress;
                isNewAddress = db.Addresses.Where(a => a.Address1 == newAddress.Address1)
                                            .Where(a => a.Address2 == newAddress.Address2)
                                            .Where(a => a.City == newAddress.City)
                                            .Where(a => a.State == newAddress.State)
                                            .Where(a => a.PostalCode == newAddress.PostalCode)
                                            .Where(a => a.Phone == newAddress.Phone)
                                            .Where(a => a.Recipient == newAddress.Phone)
                                            .Where(a => a.CustomerId == customer.CustomerId).Count() == 0;
            }
            if (isNewAddress)
            {
                shoppingCart.Order.ShipToAddress.Customer = customer;
                shoppingCart.Order.ShipToAddress.SetNullStringsToEmpty();
                db.Addresses.Add(shoppingCart.Order.ShipToAddress);
            }

            shoppingCart.Order.DateOrdered = DateTime.Now;
            db.Orders.Add(shoppingCart.Order);

            for (int i = 0; i < shoppingCart.Order.OrderDetails.Count; i++)
            {
                db.OrderDetails.Add(shoppingCart.Order.OrderDetails[i]);
            }
            db.SaveChanges();




            return this.JOk();
        }


        public ActionResult OrderSuccess()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddPromoCode()
        {
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            try
            {
                string pc = Request.Params.Get("PromoCode");
                PromoCode promoCode = db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).Single();

                shoppingCart.AddPromoCode(promoCode);

                shoppingCart.SaveToSession(HttpContext);
                return Redirect("Index");
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: Invalid promo code");
                ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
                return View("Index", shoppingCart);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: " + e.Message);
                ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
                return View("Index", shoppingCart);
            }
        }
    }
}