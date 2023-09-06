using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }

        public OrderVM(){}
        public OrderVM(OrderMDL order)
        {
            OrderId = order.OrderId; 
            UserId = order.UserId;
            OrderDate = order.OrderDate;
        }
    }
}