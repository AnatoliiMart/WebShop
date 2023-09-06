using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebShop.Models.ViewModels.Account
{
    public class OrdersForUserVM
    {
        [DisplayName("Order number")]
        public int OrderNumber { get; set; }

        [DisplayName("Total to pay")]
        public decimal Total { get; set; }

        public Dictionary<string, int> ProductsAndQty { get; set; }

        [DisplayName("Order date")]
        public DateTime OrderDate { get; set; }
    }
}