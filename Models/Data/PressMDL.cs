using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models.Data
{
    [Table("tblPresses")]
    public class PressMDL
    {
        public PressMDL()
        {
            Books = new List<BookMDL>();
        }

        [Key]
        public int Id { get; set; }
        public string PressName { get; set; }

        // Navigation property
        public virtual List<BookMDL> Books { get; set; }
    }
}