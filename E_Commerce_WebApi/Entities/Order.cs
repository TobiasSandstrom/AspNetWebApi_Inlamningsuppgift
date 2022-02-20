using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    public partial class Order
    {
        public Order()
        {
            Orderrows = new HashSet<Orderrow>();
        }

        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int OrderStatusId { get; set; }
        public int? UserId { get; set; }

        [ForeignKey(nameof(OrderStatusId))]
        [InverseProperty(nameof(Status.Orders))]
        public virtual Status OrderStatus { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Orders")]
        public virtual User User { get; set; }
        [InverseProperty(nameof(Orderrow.Order))]
        public virtual ICollection<Orderrow> Orderrows { get; set; }
    }
}
