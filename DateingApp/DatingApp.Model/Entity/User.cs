using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Model.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [StringLength(50)]
        public string UserName { get; set; }
        
        [StringLength(250)]
        public byte[] PasswordHash { get; set; }
        
        [StringLength(250)]
        public byte[] PasswordSalt { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;


        [StringLength(25)]
        public string KnownAs { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        [StringLength(2000)]
        public string Introduction { get; set; }

        [StringLength(2000)]
        public string LookingFor { get; set; }

        [StringLength(500)]
        public string Interests { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }


        public ICollection<Photo> Photos { get; set; }
    }
}
