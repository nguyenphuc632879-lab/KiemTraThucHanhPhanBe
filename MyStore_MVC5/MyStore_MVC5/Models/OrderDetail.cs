using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore_MVC5.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal")]
        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderID")]
        public virtual Orders Orders { get; set; }

        [ForeignKey("ProductID")]
        public virtual Products Products { get; set; }
    }
}
