using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Books
{
    public class GenreVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string GenreName { get; set; }
        public List<BookMDL> Books { get; set; }

        public GenreVM()
        {
        }

        public GenreVM(GenreMDL model)
        {
            Id = model.Id;
            GenreName = model.GenreName;
            Books = model.Books;
        }
    }
}