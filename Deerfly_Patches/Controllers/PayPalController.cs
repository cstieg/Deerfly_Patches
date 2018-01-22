using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.Geography;
using Cstieg.ControllerHelper;
using Cstieg.Sales.PayPal;
using Cstieg.Sales.Models;
using DeerflyPatches.Models;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class PayPalController : BaseController
    {
        private PayPalApiClient _paypalClient = new PayPalApiClient();

        /// <summary>
        /// Gets a PayPal object representation of the order in the shopping cart
        /// </summary>
        /// <returns>A JSON object in PayPal order format</returns>
        public async Task<JsonResult> GetOrderJson()
        {
            var db = new ApplicationDbContext();
            string country = Request.Params.Get("country");
            try
            {
                ShoppingCart shoppingCart = await GetShoppingCart(db);
                
                // apply country-specific charges
                shoppingCart.Country = country;
                //shoppingCart.UpdateShippingCharges();
                // ************************************* Deerflypatches.com specific ***********************************
                if (country == "US")
                {
                    shoppingCart.RemoveAllShippingCharges();
                }
                else
                {
                    shoppingCart.ResetShippingCharges();
                }

                await SaveShoppingCart(shoppingCart, db);

                string orderJson = _paypalClient.CreateOrder(shoppingCart);
                return Json(orderJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        // POST /PayPal/VerifyAndSave?paymentDetails={.....}
        /// <summary>
        /// Verifies and saves the shopping cart
        /// </summary>
        /// <param name="paymentDetails">The payment details object created by PayPal create order API call</param>
        /// <returns>Json success</returns>
        [HttpPost]
        public async Task<JsonResult> VerifyAndSave()
        {
            var db = new ApplicationDbContext();
            PaymentDetails paymentDetails = JsonConvert.DeserializeObject<PaymentDetails>(Request.Params.Get("paymentDetails"));
            ShoppingCart shoppingCart = await GetShoppingCart(db);

            // get address and add to shopping cart
            AddressBase shippingAddress = paymentDetails.Payer.PayerInfo.ShippingAddress;
            if (shoppingCart.Order.ShipToAddress == null)
            {
                shoppingCart.Order.ShipToAddress = new ShipToAddress();
            }
            shippingAddress.CopyTo(shoppingCart.Order.ShipToAddress);

            try
            {
                // verify payment details
                paymentDetails.VerifyShoppingCart(shoppingCart);
                //paymentDetails.VerifyCountry(shoppingCart, await db.Countries.ToListAsync());
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
            
            try
            { 
                // save order info to db
                await SaveShoppingCartToDbAsync(shoppingCart, paymentDetails, db);

                // clear shopping cart
                await DeleteShoppingCart(shoppingCart, db);
            }
            catch (Exception e)
            {
                return this.JError(500, "Database error");
            }

            // On success, front end will execute payment with PayPal
            return this.JOk();
        }

        /// <summary>
        /// Saves the shopping cart to the database.
        /// </summary>
        /// <param name="shoppingCart">Shopping cart stored in session</param>
        /// <param name="paymentDetails">Payment details received from PayPal API</param>
        /// <param name="db">Database context to which shopping cart belongs</param>
        private async Task SaveShoppingCartToDbAsync(ShoppingCart shoppingCart, PaymentDetails paymentDetails, ApplicationDbContext db)
        {
            // update customer ------------------------------------------------------
            DateTime currentTime = DateTime.Now;
            PayerInfo payerInfo = paymentDetails.Payer.PayerInfo;
            Customer customer = await db.Customers.SingleOrDefaultAsync(c => c.EmailAddress == payerInfo.Email);
            bool isNewCustomer = customer == null;
            if (isNewCustomer)
            {
                customer = new Customer()
                {
                    Registered = currentTime,
                    LastVisited = currentTime,
                    EmailAddress = payerInfo.Email,
                    FirstName = payerInfo.FirstName,
                    LastName = payerInfo.LastName
                };
                db.Customers.Add(customer);
            }
            else
            {
                shoppingCart.Order.Customer = customer;
                shoppingCart.Order.CustomerId = customer.Id;
                customer.LastVisited = currentTime;
                db.Entry(customer).State = EntityState.Modified;
            }

            // update address ----------------------------------------------------------
            bool isNewAddress = true;
            if (!isNewCustomer)
            {
                AddressBase newAddress = payerInfo.ShippingAddress;
                newAddress.SetNullStringsToEmpty();
                ShipToAddress addressOnFile = await db.Addresses.Where(a => a.Address1 == newAddress.Address1
                                                            && a.Address2 == newAddress.Address2
                                                            && a.City == newAddress.City
                                                            && a.State == newAddress.State
                                                            && a.PostalCode == newAddress.PostalCode
                                                            && a.Phone == newAddress.Phone
                                                            && a.Recipient == newAddress.Recipient
                                                            && a.CustomerId == customer.Id).FirstOrDefaultAsync();
                isNewAddress = addressOnFile == null;

                // don't add new address if already in database
                if (!isNewAddress)
                {
                    shoppingCart.Order.ShipToAddressId = addressOnFile.Id;
                    if (shoppingCart.Order.ShipToAddress != null)
                    {
                        db.Entry(shoppingCart.Order.ShipToAddress).State = EntityState.Detached;
                    }
                    if (shoppingCart.Order.BillToAddress != null)
                    {
                        db.Entry(shoppingCart.Order.BillToAddress).State = EntityState.Detached;
                    }
                }
            }

            shoppingCart.Order.ShipToAddress.SetNullStringsToEmpty();

            // Add new address to database
            if (isNewAddress)
            { 
                shoppingCart.Order.ShipToAddress.Customer = customer;
                db.Addresses.Add(shoppingCart.Order.ShipToAddress);
            }

            // bill to address the same as shipping address
            if (shoppingCart.Order.BillToAddress == null || shoppingCart.Order.BillToAddress.Address1 == "")
            {
                shoppingCart.Order.BillToAddressId = shoppingCart.Order.ShipToAddressId;
            }

            // add order to database --------------------------------------------------
            shoppingCart.Order.Cart = paymentDetails.Cart;
            shoppingCart.Order.DateOrdered = currentTime;
            db.Entry(shoppingCart.Order).State = EntityState.Modified;
            // order details are already in db as part of shopping cart

            await db.SaveChangesAsync();
        }

    }
}