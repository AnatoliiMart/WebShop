﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop.Models.Data
{
    [Table("tblOrderDetails")]
    public class OrderDetailsMDL
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderMDL Order { get; set; }

        [ForeignKey("UserId")]
        public virtual UserMDL User { get; set;}

        [ForeignKey("ProductId")]
        public virtual ProductMDL Product { get; set;}
    }
}