using Deerfly_Patches.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Hosting;

namespace Deerfly_Patches.Modules.PayPal
{
    /// <summary>
    /// Client for PayPal API
    /// </summary>
    public class PayPalApiClient
    {
        public ClientInfo ClientInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string PayeeEmail { get; set; }

        /// <summary>
        /// Constructor for PayPalApiClient which loads urls from paypal.json
        /// </summary>
        public PayPalApiClient()
        {
            ClientInfo = GetClientSecrets();
            ReturnUrl = ClientInfo.ReturnUrl;
            CancelUrl = ClientInfo.CancelUrl;
            PayeeEmail = ClientInfo.ClientAccount;
        }

        /// <summary>
        /// Gets client id and secret from PayPal.json in root directory
        /// </summary>
        /// <returns>ClientInfo object containing client id info</returns>
        public ClientInfo GetClientSecrets()
        {
            string file = HostingEnvironment.MapPath("/PayPal.json");
            string json = System.IO.File.ReadAllText(file);
            ClientInfo paypalSecrets = JsonConvert.DeserializeObject<ClientInfo>(json);
            return paypalSecrets;
        }
        
        /// <summary>
        /// Creates an order to pass to PayPal API
        /// </summary>
        /// <param name="shoppingCart">Shopping cart object containing items to put in order</param>
        /// <returns>JSON representation of the order in the format expected by PayPal</returns>
        public string CreateOrder(ShoppingCart shoppingCart)
        {
            // Create description of order
            string description;
            switch (shoppingCart.Order.OrderDetails.Count)
            {
                case 0:
                    throw new ArgumentException("Cannot create an order from an empty shopping cart!");
                case 1:
                    description = shoppingCart.Order.OrderDetails[0].Product.Name + " - Qty: " + shoppingCart.Order.OrderDetails[0].Quantity;
                    break;
                default:
                    description = "Multiple products";
                    break;
            }

            // Create JSON order object
            PaymentDetails data = new PaymentDetails()
            {
                Intent = "order",
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                },
                Transactions = new List<Transaction>
                {
                    new Transaction()
                    {
                        Amount = new Amount
                        {
                            Currency = "USD",
                            Total = shoppingCart.GrandTotal,
                            AmountDetails = new AmountDetails
                            {
                                Shipping = shoppingCart.TotalShipping,
                                Subtotal = shoppingCart.TotalExtendedPrice,
                                Tax = 0
                            }
                        },
                        Payee = new Payee()
                        {
                            Email = PayeeEmail
                        },
                        Description = description,
                        ItemList = new ItemList()
                        {
                            Items = GetPayPalItems(shoppingCart),
                            //shipping_address = GetPayPalAddress(shoppingCart.GetOrder().ShipToAddress)
                        }
                    }
                },
                RedirectUrls = new RedirectUrls
                {
                    ReturnUrl = ReturnUrl,
                    CancelUrl = CancelUrl
                }
            };
            string dataJSON = JsonConvert.SerializeObject(data);
            return dataJSON;
        }

        /// <summary>
        /// Converts address to PayPal object format
        /// </summary>
        /// <param name="address">Address to convert</param>
        /// <returns>Object representation of address in PayPal format</returns>
        private ShippingAddress GetPayPalAddress(Address address)
        {
            return new ShippingAddress()
            {
                Recipient = address.Recipient,
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                Phone = address.Phone
            };
        }

        /// <summary>
        /// Converts OrderDetail model to PayPal item format
        /// </summary>
        /// <param name="orderDetail">OrderDetail object containing item being purchased</param>
        /// <returns>Purchase item in PayPal object format</returns>
        private Item GetPayPalItem(OrderDetail orderDetail)
        {
            return new Item
            {
                Name = orderDetail.Product.Name,
                Quantity = orderDetail.Quantity,
                Price = orderDetail.UnitPrice,
                Sku = orderDetail.Product.ProductId.ToString(),
                Currency = "USD"
            };
        }

        /// <summary>
        /// Converts shopping cart info to PayPal object format
        /// </summary>
        /// <param name="shoppingCart">Shopping cart containing items to be purchased</param>
        /// <returns>Shopping cart items in PayPal object format</returns>
        private List<Item> GetPayPalItems(ShoppingCart shoppingCart)
        {
            List<Item> items = new List<Item>();
            List<OrderDetail> shoppingCartItems = shoppingCart.GetItems();

            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                items.Add(GetPayPalItem(shoppingCartItems[i]));
            }
            return items;
        }
    }
}
