using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStore_MVC5.Models
{
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
