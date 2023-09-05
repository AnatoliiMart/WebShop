using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Books
{
    public class AuthorVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Author name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Author surname")]
        public string LastName { get; set; }

        public ICollection<BookMDL> Books { get; set; }

        public AuthorVM() { }

        public AuthorVM(AuthorMDL model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Books = model.Books;
        }
    }
}