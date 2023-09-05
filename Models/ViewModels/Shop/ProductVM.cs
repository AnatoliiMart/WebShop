using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Shop
{
    public class ProductVM
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Author { get; set; }

        [Required]
        [StringLength(50)]
        public string Press { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Genre { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Description { get; set; }

        public double Price { get; set; }


        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [Required]
        [DisplayName("Book")]
        public int BookId { get; set; }

        [DisplayName("Image")]
        public string ImageName { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<SelectListItem> Books { get; set; }

        public IEnumerable<String> GalleryImages { get; set; }

        public ProductVM() { }

        public ProductVM(ProductMDL product) 
        {
            Id = product.Id;
            Title = product.Title;
            Author = product.Author;
            Genre = product.Genre;
            Press = product.Press;
            Description = product.Description;
            Price = product.Price;
            CategoryName = product.CategoryName;
            CategoryId = product.CategoryId;
            BookId = product.BookId;
            ImageName = product.ImageName;
        }
    }
}