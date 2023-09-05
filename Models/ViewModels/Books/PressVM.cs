using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Books
{
    public class PressVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string PressName { get; set; }
        
        public List<BookMDL> Books { get; set; }

        public PressVM() { }

        public PressVM(PressMDL model)
        {
            Id = model.Id;
            PressName = model.PressName;
            Books = new List<BookMDL>();
        }
    }
}