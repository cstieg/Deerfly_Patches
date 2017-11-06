using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Deerfly_Patches.Modules.Geography;


namespace Deerfly_Patches.Modules.PayPal
{
    [DataContract]
    public class PaymentDetails
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "create_time")]
        public DateTime CreateTime { get; set; }

        [DataMember(Name = "cart")]
        public string Cart { get; set; }

        [DataMember(Name = "intent")]
        public string Intent { get; set; }

        [DataMember(Name = "payer")]
        public Payer Payer { get; set; }

        [DataMember(Name = "transactions")]
        public IEnumerable<Transaction> Transactions { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "redirect_urls")]
        public RedirectUrls RedirectUrls { get; set; }

        public bool ShouldSerializeId()
        {
            return Id != null;
        }

        public bool ShouldSerializeCreateTime()
        {
            return CreateTime != null && CreateTime != DateTime.MinValue;
        }

        public bool ShouldSerializeCart()
        {
            return Cart != null && Cart != "";
        }

        public bool ShouldSerializeState()
        {
            return State != null && State != "";
        }


        /// <summary>
        /// Protect against malicious changing of shopping cart items before payment was formed 
        /// by checking items against items in shoppingCart on server
        /// </summary>
        /// <param name="shoppingCart">Shopping cart on server</param>
        public void VerifyShoppingCart(ShoppingCart shoppingCart)
        {
            // verify items
            List<Item> items = (List<Item>)Transactions.First().ItemList.Items;
            for (int i = 0; i < items.Count(); i++)
            {
                if (!shoppingCart.Order.OrderDetails.Exists(o => o.Product.ProductId == int.Parse(items[i].Sku) &&
                    o.Quantity == items[i].Quantity &&
                    o.UnitPrice == items[i].Price))
                {
                    throw new ArgumentException("Your shopping cart has been changed!  Please try again.");
                }
            }

            if (Transactions.First().Amount.Total != shoppingCart.GrandTotal)
            {
                throw new ArgumentException("Your shopping cart has been changed! Please try again.");
            }
        }
    }

    [DataContract]
    public class Payer
    {
        [DataMember(Name = "payment_method")]
        public string PaymentMethod { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "payer_info")]
        public PayerInfo PayerInfo { get; set; }

        public bool ShouldSerializeStatus()
        {
            return Status != null;
        }

        public bool ShouldSerializePayerInfo()
        {
            return PayerInfo != null;
        }
    }

    [DataContract]
    public class PayerInfo
    {
        [DataMember(Name = "payer_id")]
        public string PayerId { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "middle_name")]
        public string MiddleName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }

        [DataMember(Name = "shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }

    [DataContract]
    public class ShippingAddress : AddressBase
    {
        [DataMember(Name = "recipient_name")]
        public override string Recipient { get; set; }

        [DataMember(Name = "line1")]
        public override string Address1 { get; set; }

        [DataMember(Name = "line2")]
        public override string Address2 { get; set; }

        [DataMember(Name = "city")]
        public override string City { get; set; }

        [DataMember(Name = "state")]
        public override string State { get; set; }

        [DataMember(Name = "postal_code")]
        public override string PostalCode { get; set; }

        [DataMember(Name = "country_code")]
        public override string Country { get; set; }

        [DataMember(Name = "phone")]
        public override string Phone { get; set; }
    }
    
    [DataContract]
    public class Transaction
    {
        [DataMember(Name = "amount")]
        public Amount Amount { get; set; }

        [DataMember(Name = "payee")]
        public Payee Payee { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        [DataMember(Name = "item_list")]
        public ItemList ItemList { get; set; }
    }

    [DataContract]
    public class Amount
    {
        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "total")]
        public decimal Total { get; set; }

        [DataMember(Name = "details")]
        public AmountDetails AmountDetails { get; set; }
    }

    [DataContract]
    public class AmountDetails
    {
        [DataMember(Name = "shipping")]
        public decimal Shipping { get; set; }

        [DataMember(Name = "subtotal")]
        public decimal Subtotal { get; set; }

        [DataMember(Name = "tax")]
        public decimal Tax { get; set; }
    }

    [DataContract]
    public class Payee
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }

    [DataContract]
    public class ItemList
    {
        [DataMember(Name = "items")]
        public IEnumerable<Item> Items { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }
    }

    [DataContract]
    public class RedirectUrls
    {
        [DataMember(Name = "return_url")]
        public string ReturnUrl { get; set; }

        [DataMember(Name = "cancel_url")]
        public string CancelUrl { get; set; }
    }
}