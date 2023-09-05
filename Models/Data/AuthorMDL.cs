using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models.Data
{
    [Table("tblAuthors")]
    public class AuthorMDL
    {
        public AuthorMDL() => Books = new List<BookMDL>();
  
        [Key]
        public int Id { get; set; }

        [Display(Name = "Author name")]
        public string FirstName { get; set; }

        [Display(Name = "Author surname")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get; set; }
        // Navigation property
        public virtual List<BookMDL> Books { get; set; }
    }
}