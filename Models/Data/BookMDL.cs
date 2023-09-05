using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop.Models.Data
{
    [Table("tblBooks")]
    public class BookMDL
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        // Foreign keys
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public int PressId { get; set; }

        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual AuthorMDL Author { get; set; }

        [ForeignKey("GenreId")]
        public virtual GenreMDL Genre { get; set; }

        [ForeignKey("PressId")]
        public virtual PressMDL Press { get; set; }
    }
}