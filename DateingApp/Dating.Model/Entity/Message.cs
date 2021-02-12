using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dating.Model.Entity
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SenderId { get; set; }

        [StringLength(50)]
        public string SenderUsername { get; set; }

        public int Recipientid { get; set; }

        [StringLength(50)]
        public string RecipientUsername { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; } = DateTime.UtcNow;

        public bool SenderDeleted { get; set; }

        public bool RecipientDeleted { get; set; }

        public User Sender { get; set; }
        public User Recipient { get; set; }
    }
}
