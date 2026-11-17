using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStore_MVC5.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
