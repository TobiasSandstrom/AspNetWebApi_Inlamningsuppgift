using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Entities
{
    public partial class Hash
    {
        public Hash()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public byte[] Pass { get; set; }
        [Required]
        public byte[] Salt { get; set; }

        [InverseProperty(nameof(User.Hash))]
        public virtual ICollection<User> Users { get; set; }
    }
}
