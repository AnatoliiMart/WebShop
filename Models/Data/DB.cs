using System.Data.Entity;

namespace WebShop.Models.Data
{
    public class DB : DbContext
    {
        public DB() { }

        //Default
        public DbSet<PageMDL> Pages { get; set; }
        public DbSet<SidebarMDL> Sidebars { get; set; }
        public DbSet<CategoryMDL> Categories { get; set; }

        //For exam work
        public DbSet<BookMDL> Books { get; set; }
        public DbSet<AuthorMDL> Authors { get; set; }
        public DbSet<GenreMDL> Genres { get; set; }
        public DbSet<PressMDL> Presses { get; set; }
        public DbSet<ProductMDL> Products { get; set; }

        //Accounts base
        public DbSet<UserMDL> Users { get; set; }
        public DbSet<RoleMDL> Roles { get; set; }
        public DbSet<UserRoleMDL> UserRoles { get; set; }

        //Order base
        public DbSet<OrderMDL> Orders { get; set; }
        public DbSet<OrderDetailsMDL> OrdersDetails { get; set; }


    }
}