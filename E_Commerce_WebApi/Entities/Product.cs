using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    [Index(nameof(Artnumber), Name = "UQ__Products__9A3DCBE728CBB11A", IsUnique = true)]
    public partial class Product
    {
        public Product()
        {
            Orderrows = new HashSet<Orderrow>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Artnumber { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Products")]
        public virtual Category Category { get; set; }
        [InverseProperty(nameof(Orderrow.Product))]
        public virtual ICollection<Orderrow> Orderrows { get; set; }
    }
}
