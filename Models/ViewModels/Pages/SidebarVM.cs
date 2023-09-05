using System.Web.Mvc;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Pages
{
    public class SidebarVM
    {
        public SidebarVM() { }

        public SidebarVM(SidebarMDL sidebar)
        {
            Id = sidebar.Id;
            Body = sidebar.Body;
        }

        public int Id { get; set; }

        [AllowHtml]
        public string Body { get; set; }
    }
}