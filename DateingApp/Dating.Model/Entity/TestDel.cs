using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Dating.Model.Entity
{
    class TestDel
    {
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string BillNumber { get; set; }

        [Required]
        [StringLength(25)]
        public int Name { get; set; }

        [Required]
        [StringLength(200)]
        public int Address { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }

        public ICollection<Product> Photos { get; set; }
    }

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string ProductName { get; set; }

        [Required]
        public int Quntity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
    }

}
