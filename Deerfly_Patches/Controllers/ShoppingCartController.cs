using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

using Cstieg.ControllerHelper;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.Geography;
using Deerfly_Patches.Modules.PayPal;
using Deerfly_Patches.Models;

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


        /// <summary>
        /// Verifies and saves the shopping cart
        /// </summary>
        /// <returns>Json success code</returns>
        [HttpPost]
        public async Task<JsonResult> VerifyAndSave()
        {
            string paymentDetailsJson = Request.Params.Get("paymentDetails");
            PaymentDetails paymentDetails = JsonConvert.DeserializeObject<PaymentDetails>(paymentDetailsJson);
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // get address and add to shopping cart
            AddressBase shippingAddress = paymentDetails.Payer.PayerInfo.ShippingAddress;
            shippingAddress.CopyTo(shoppingCart.Order.ShipToAddress);

            try
            {
                paymentDetails.VerifyShoppingCart(shoppingCart);
                CustomVerification(shoppingCart, paymentDetails);

                await SaveShoppingCartToDbAsync(shoppingCart, paymentDetails.Payer.PayerInfo);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }

            // clear shopping cart
            shoppingCart = new ShoppingCart();
            shoppingCart.SaveToSession(HttpContext);

            // return success
            return this.JOk();
        }

        /// <summary>
        /// Saves the shopping cart to the database.
        /// </summary>
        /// <param name="shoppingCart">Shopping cart stored in session</param>
        /// <param name="payerInfo">Payer info received from PayPal API</param>
        protected async Task SaveShoppingCartToDbAsync(ShoppingCart shoppingCart, PayerInfo payerInfo)
        {
            var customersDb = db.Customers;
            var addressesDb = db.Addresses;
            var ordersDb = db.Orders;

            // update customer
            Customer customer = await customersDb.SingleOrDefaultAsync(c => c.EmailAddress == payerInfo.Email);
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
                customersDb.Add(customer);
            }
            else
            {
                db.Entry(customer).State = EntityState.Modified;
            }

            // update address
            bool isNewAddress = true;
            if (!isNewCustomer)
            {
                AddressBase newAddress = payerInfo.ShippingAddress;
                newAddress.SetNullStringsToEmpty();
                Address addressOnFile = await addressesDb.Where(a => a.Address1 == newAddress.Address1
                                                            && a.Address2 == newAddress.Address2
                                                            && a.City == newAddress.City
                                                            && a.State == newAddress.State
                                                            && a.PostalCode == newAddress.PostalCode
                                                            && a.Phone == newAddress.Phone
                                                            && a.Recipient == newAddress.Recipient
                                                            && a.CustomerId == customer.CustomerId).FirstOrDefaultAsync();
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
                addressesDb.Add(shoppingCart.Order.ShipToAddress);
            }

            // bill to address the same as shipping address
            if (shoppingCart.Order.BillToAddress == null || shoppingCart.Order.BillToAddress.Address1 == null)
            {
                shoppingCart.Order.BillToAddress = shoppingCart.Order.ShipToAddress;
                shoppingCart.Order.BillToAddressId = shoppingCart.Order.ShipToAddressId;
            }

            await db.SaveChangesAsync();

            // don't add duplicate of product
            for (int i = 0; i < shoppingCart.Order.OrderDetails.Count; i++)
            {
                var orderDetail = shoppingCart.Order.OrderDetails[i];
                orderDetail.ProductId = orderDetail.Product.ProductId;
                db.Entry(orderDetail.Product).State = EntityState.Unchanged;
            }

            // add order to database
            shoppingCart.Order.DateOrdered = DateTime.Now;
            ordersDb.Add(shoppingCart.Order);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Custom verification of shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart stored in session</param>
        /// <param name="paymentDetails">Payment details received from PayPal API</param>
        protected void CustomVerification(ShoppingCart shoppingCart, PaymentDetails paymentDetails)
        {
            AddressBase shippingAddress = paymentDetails.Payer.PayerInfo.ShippingAddress;
            if ((shoppingCart.Country == "US" && shippingAddress.Country != "US") ||
                (shoppingCart.Country != "US" && shippingAddress.Country == "US"))
            {
                // change to JSON error
                throw new ArgumentException("Your country does not match the country selected!");
            }
        }
        

        public ActionResult OrderSuccess()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddPromoCode()
        {
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            try
            {
                string pc = Request.Params.Get("PromoCode");
                PromoCode promoCode = await db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).SingleAsync();

                shoppingCart.AddPromoCode(promoCode);

                shoppingCart.SaveToSession(HttpContext);
                return Redirect("Index");
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: Invalid promo code - " + e.Message);
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