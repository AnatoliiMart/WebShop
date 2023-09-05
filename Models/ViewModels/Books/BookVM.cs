using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Books
{
    public class BookVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        public int PublicationYear { get; set; }

        // Foreign keys
        [Required]
        [DisplayName("Author")]
        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        [Required]
        [DisplayName("Genre")]
        public int GenreId { get; set; }

        public string GenreName { get; set; }

        [Required]
        [DisplayName("Press")]
        public int PressId { get; set; }

        public string PressName { get; set; }

        public IEnumerable<SelectListItem> Authors { get; set; }
        public IEnumerable<SelectListItem> Genres { get; set; }
        public IEnumerable<SelectListItem> Presses { get; set; }

        public BookVM() { }

        public BookVM(BookMDL model)
        {
            Id = model.Id;
            Title = model.Title;
            PublicationYear = model.PublicationYear;
            AuthorId = model.AuthorId;
            GenreId = model.GenreId;
            PressId = model.PressId;
        }
    }
}