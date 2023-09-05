using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Pages
{
    public class PageVM
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }

        public int Sorting { get; set; }

        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }

        public PageVM(PageMDL page)
        {
            Id = page.Id;
            Title = page.Title;
            Slug = page.Slug;
            Body = page.Body;
            Sorting = page.Sorting;
            HasSidebar = page.HasSidebar;
        }

        public PageVM() { }
    }
}