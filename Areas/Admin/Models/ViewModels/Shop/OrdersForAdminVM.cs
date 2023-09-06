﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebShop.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        [DisplayName("Order number")]
        public int OrderNumber { get; set; }

        
        public string UserName { get; set; }
        [DisplayName("Total to pay")]
        public decimal Total { get; set; }

        public Dictionary<string, int> ProductsAndQty { get; set; }

        [DisplayName("Order date")]
        public DateTime OrderDate { get; set; }
    }
}