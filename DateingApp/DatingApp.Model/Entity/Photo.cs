using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Model.Entity
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(250)]
        public string Url { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public bool IsMain { get; set; }

        [StringLength(250)]
        public string PublicId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
