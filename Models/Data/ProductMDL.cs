using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop.Models.Data
{
    [Table("tblProducts")]
    public class ProductMDL
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

        public string Press { get; set; }

        public string Genre { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }

        public string CategoryName { get; set; }

        public int CategoryId { get; set; }

        public int BookId { get; set; }

        public string ImageName { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryMDL Category { get; set; }

        [ForeignKey("BookId")]
        public virtual BookMDL Book { get; set; }
    }
}