using System;
using System.Collections.Generic;
using System.Linq;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Modules.PayPal
{
    public class ShoppingCart
    {
        public Order Order;
        public List<OrderDetail> OrderDetails { get; set; }
        public string PayeeEmail { get; set; }
        public decimal TotalExtendedPrice
        {
            get
            {
                return OrderDetails.Sum(p => p.ExtendedPrice);
            }
        }

        public decimal TotalShipping
        {
            get
            {
                return OrderDetails.Sum(p => p.Shipping);
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
            OrderDetails = Order.OrderDetails;
        }

        public List<OrderDetail> GetItems()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return OrderDetails;
        }

        public Order GetOrder()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return Order;
        }

        public void AddOrderDetail(OrderDetail newItem)
        {
            OrderDetails.Add(newItem);
        }

        public void AddProduct(Product product)
        {
            OrderDetail orderDetail = OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
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
                OrderDetails.Add(orderDetail);
            }
            else
            {
                orderDetail.Quantity++;
            }
        }

        public void DecrementProduct(Product product)
        {
            OrderDetail orderDetail = OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
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
            OrderDetail orderDetail = OrderDetails.Find(p => p.Product.ProductId == product.ProductId);
            if (!(orderDetail == null))
            {
                OrderDetails.Remove(orderDetail);
            }
        }
        
    }
}
