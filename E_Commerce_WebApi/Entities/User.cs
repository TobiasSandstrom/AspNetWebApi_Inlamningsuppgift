using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    [Index(nameof(Email), Name = "UQ__Users__A9D105345D8B37FF", IsUnique = true)]
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Firstname { get; set; }
        [StringLength(50)]
        public string Lastname { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Email { get; set; }
        public int? AddressId { get; set; }
        public int? HashId { get; set; }
        public int? RoleId { get; set; }

        [ForeignKey(nameof(AddressId))]
        [InverseProperty("Users")]
        public virtual Address Address { get; set; }
        [ForeignKey(nameof(HashId))]
        [InverseProperty("Users")]
        public virtual Hash Hash { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("Users")]
        public virtual Role Role { get; set; }
        [InverseProperty(nameof(Order.User))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
