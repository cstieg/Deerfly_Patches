using System;
using System.Collections.Generic;
using System.Linq;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Modules.PayPal
{
    public class ShoppingCart
    {
        public Order Order;
        public string PayeeEmail { get; set; }
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

        public ShoppingCart()
        {
            Order = new Order
            {
                BillToAddress = new Address(),
                ShipToAddress = new Address(),
                OrderDetails = new List<OrderDetail>()
            };
        }

        public List<OrderDetail> GetItems()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return Order.OrderDetails;
        }

        public Order GetOrder()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return Order;
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
        }
        
    }
}
