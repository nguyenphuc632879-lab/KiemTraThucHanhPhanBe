using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore_MVC5.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        [Required, StringLength(50)]
        public string PaymentStatus { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required, StringLength(50)]
        public string ShippingMethod { get; set; }

        [Required, StringLength(255)]
        public string ShippingAddress { get; set; }

        [Required, StringLength(50)]
        public string OrderStatus { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customers Customers { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
