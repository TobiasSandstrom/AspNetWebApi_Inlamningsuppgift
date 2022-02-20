using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    public partial class Status
    {
        public Status()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Orderstatus { get; set; }

        [InverseProperty(nameof(Order.OrderStatus))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
