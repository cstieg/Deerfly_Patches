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
            List<Item> items = (List<Item>)paymentDetails.Transactions.First().ItemList.Items;
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
                    EmailAddress = payerInfo.Email,
                    CustomerName = payerInfo.FirstName + " " +
                                    payerInfo.MiddleName + " " +
                                    payerInfo.LastName
                };
            }
            else
            {
                shoppingCart.Order.Customer = customer;
                shoppingCart.Order.CustomerId = customer.CustomerId;
            }


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
                newAddress.SetNullStringsToEmpty();
                Address addressOnFile = db.Addresses.Where(a => a.Address1 == newAddress.Address1 
                                                            && a.Address2 == newAddress.Address2
                                                            && a.City == newAddress.City
                                                            && a.State == newAddress.State
                                                            && a.PostalCode == newAddress.PostalCode
                                                            && a.Phone == newAddress.Phone
                                                            && a.Recipient == newAddress.Recipient
                                                            && a.CustomerId == customer.CustomerId).FirstOrDefault();
                isNewAddress = addressOnFile == null;

                // don't add new address if already in database
                if (!isNewAddress)
                {
                    shoppingCart.Order.ShipToAddressId = addressOnFile.AddressId;
                }
            }

            shoppingCart.Order.ShipToAddress.Customer = customer;
            shoppingCart.Order.ShipToAddress.CustomerId = customer.CustomerId;
            shoppingCart.Order.ShipToAddress.SetNullStringsToEmpty();

            // Add new address to database
            if (isNewAddress)
            {
                db.Addresses.Add(shoppingCart.Order.ShipToAddress);
            }

            // bill to address the same as shipping address
            if (shoppingCart.Order.BillToAddress == null || shoppingCart.Order.BillToAddress.Address1 == null)
            {
                shoppingCart.Order.BillToAddress = shoppingCart.Order.ShipToAddress;
                shoppingCart.Order.BillToAddressId = shoppingCart.Order.ShipToAddressId;
            }

            db.SaveChanges();

            // don't add duplicate of product
            for (int i = 0; i < shoppingCart.Order.OrderDetails.Count; i++)
            {
                var orderDetail = shoppingCart.Order.OrderDetails[i];
                orderDetail.ProductId = orderDetail.Product.ProductId;
                db.Entry(orderDetail.Product).State = EntityState.Unchanged;
            }

            // add order to database
            shoppingCart.Order.DateOrdered = DateTime.Now;
            db.Orders.Add(shoppingCart.Order);

            db.SaveChanges();

            // clear shopping cart
            shoppingCart = new ShoppingCart();
            shoppingCart.SaveToSession(HttpContext);

            // return success
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