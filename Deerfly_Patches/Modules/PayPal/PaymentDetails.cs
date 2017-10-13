using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
    public class ShippingAddress
    {
        [DataMember(Name = "recipient_name")]
        public string Recipient { get; set; }

        [DataMember(Name = "line1")]
        public string Address1 { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "postal_code")]
        public string PostalCode { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }
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
}