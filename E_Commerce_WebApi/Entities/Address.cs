using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    public partial class Address
    {
        public Address()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Street { get; set; }
        [StringLength(5)]
        public string Zipcode { get; set; }
        [StringLength(50)]
        public string City { get; set; }

        [InverseProperty(nameof(User.Address))]
        public virtual ICollection<User> Users { get; set; }
    }
}
