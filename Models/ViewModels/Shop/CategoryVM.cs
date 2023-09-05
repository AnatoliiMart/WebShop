using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM() { }

        public CategoryVM(CategoryMDL model)
        {
            Id = model.Id;
            Name = model.Name;
            Slug = model.Slug;
            Sorting = model.Sorting;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int Sorting { get; set; }
    }
}