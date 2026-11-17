using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore_MVC5.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        public int CategoryID { get; set; }

        [Required, StringLength(200)]
        public string ProductName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Image { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Categories Categories { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
