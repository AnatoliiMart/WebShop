using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models.Data
{
    [Table("tblGenres")]
    public class GenreMDL
    {
        public GenreMDL() => Books = new List<BookMDL>();
        

        [Key]
        public int Id { get; set; }
        public string GenreName { get; set; }
        // Navigation property
        public virtual List<BookMDL> Books { get; set; }

    }
}