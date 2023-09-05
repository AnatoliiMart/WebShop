using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop.Models.Data
{
    [Table("tblSidebar")]
    public class SidebarMDL
    {
        [Key]
        public int Id { get; set; }
       
        public string Body { get; set; }
    }
}