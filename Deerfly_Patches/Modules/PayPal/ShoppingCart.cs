using System;
using System.Collections.Generic;
using System.Linq;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Modules.PayPal
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Order = new Order
            {
                BillToAddress = new Address(),
                ShipToAddress = new Address(),
                OrderDetails = new List<OrderDetail>()
            };
            PromoCodes = new List<PromoCode>();
        }

        public Order Order { get; set; }

        public string PayeeEmail { get; set; }

        public List<PromoCode> PromoCodes { get; set; }

        public decimal TotalExtendedPrice
        {
            get
            {
                return Order.OrderDetails.Sum(p => p.ExtendedPrice);
            }
        }

        public decimal TotalShipping
        {
            get
            {
                return Order.OrderDetails.Sum(p => p.Shipping);
            }
        }

        public decimal GrandTotal
        {
            get
            {
                return TotalExtendedPrice + TotalShipping;
            }
        }

        public List<OrderDetail> GetItems()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return Order.OrderDetails;
        }

        public void AddOrderDetail(OrderDetail newItem)
        {
            Order.OrderDetails.Add(newItem);
        }

        public void AddProduct(Product product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
            if (orderDetail == null)
            {
                orderDetail = new OrderDetail()
                {
                    Product = product,
                    PlacedInCart = DateTime.Now,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    Shipping = product.Shipping
                };
                Order.OrderDetails.Add(orderDetail);
            }
            else
            {
                orderDetail.Quantity++;
            }
        }

        public void DecrementProduct(Product product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
            if (!(orderDetail == null) && orderDetail.Quantity > 0)
            {
                orderDetail.Quantity--;
            }
            if (orderDetail.Quantity == 0)
            {
                RemoveProduct(product);
            }
        }

        public void RemoveProduct(Product product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
            if (!(orderDetail == null))
            {
                Order.OrderDetails.Remove(orderDetail);
            }

            if (Order.OrderDetails.Count == 0)
            {
                PromoCodes.Clear();
            }
        }

        public void AddPromoCode(PromoCode promoCode)
        {
            // check constraints
            if (PromoCodes.Exists(p => p.Code == promoCode.Code))
            {
                throw new Exception("Cannot enter same promo code twice!");
            }

            if (promoCode.SpecialPriceItem != null && !Order.OrderDetails.Exists(o => o.ProductId == promoCode.SpecialPriceItem.ProductId))
            {
                throw new Exception("Must purchase special item to receive discount: " + promoCode.SpecialPriceItem.Name);
            }

            if (promoCode.WithPurchaseOf != null && !Order.OrderDetails.Exists(o => o.ProductId == promoCode.WithPurchaseOf.ProductId))
            {
                throw new Exception("Must purchase qualifying item: " + promoCode.WithPurchaseOf.Name);
            }

            if (promoCode.MinimumQualifyingPurchase != null && TotalExtendedPrice < promoCode.MinimumQualifyingPurchase)
            {
                throw new Exception("Must have a minimum purchase of $" + promoCode.MinimumQualifyingPurchase.ToString());
            }

            if (promoCode.CodeStart != null && DateTime.Now < promoCode.CodeStart)
            {
                DateTime codeStart = (DateTime)promoCode.CodeStart;
                throw new Exception("Code is not valid until " + codeStart.ToShortDateString());
            }

            if (promoCode.CodeEnd != null && DateTime.Now > promoCode.CodeEnd)
            {
                throw new Exception("Code is expired!");
            }

            // Apply promotions
            if (promoCode.PromotionalItem != null)
            {
                Order.OrderDetails.Add(new OrderDetail()
                {
                    Product = promoCode.PromotionalItem,
                    UnitPrice = promoCode.PromotionalItemPrice ?? 0,
                    Quantity = 1,
                    CheckedOut = false,
                    PlacedInCart = DateTime.Now,
                    Shipping = 0
                });
            }

            // special price may be zero; only apply if a percentage is not given
            if ((promoCode.PercentOffItem == null || promoCode.PercentOffItem == 0) && promoCode.SpecialPriceItem != null)
            {
                OrderDetail item = Order.OrderDetails.Find(o => o.Product.ProductId == promoCode.SpecialPriceItem.ProductId);
                item.UnitPrice = promoCode.SpecialPrice ?? 0;
            }

            // percent off item
            if (promoCode.PercentOffItem != null && promoCode.PercentOffItem > 0 && promoCode.SpecialPriceItem != null)
            {
                OrderDetail item = Order.OrderDetails.Find(o => o.Product.ProductId == promoCode.SpecialPriceItem.ProductId);
                item.UnitPrice = PercentOff(item.UnitPrice, promoCode.PercentOffItem ?? 0);
            }

            // percent off order
            if (promoCode.PercentOffOrder != null && promoCode.PercentOffOrder > 0)
            {
                for (int i = 0; i < Order.OrderDetails.Count; i++)
                {
                    OrderDetail item = Order.OrderDetails[i];
                    item.UnitPrice = PercentOff(item.UnitPrice, promoCode.PercentOffOrder ?? 0);
                }
            }

            // Add promocode to list
            PromoCodes.Add(promoCode);
        }

        private decimal PercentOff(decimal originalPrice, decimal percentOff)
        {
            if (percentOff == 100)
            {
                return 0;
            }
            return originalPrice * (100 - percentOff) / 100;
        }

        private decimal PercentOff(decimal originalPrice, int percentOff)
        {
            return PercentOff(originalPrice, (decimal) percentOff);
        }
        
        
    }
}
