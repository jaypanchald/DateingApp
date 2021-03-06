﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Model.Entity
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int Recipientid { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }

        public bool SenderDeleted { get; set; }

        public bool RecipientDeleted { get; set; }

        public User Sender { get; set; }
        public User Recipient { get; set; }
    }
}
