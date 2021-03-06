﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Model.Message
{
    public class MessageForCreateaDto
    {

        public int SenderId { get; set; }

        public int RecipientId { get; set; }

        public DateTime MessageSent { get; set; }

        public string Content { get; set; }

        public MessageForCreateaDto()
        {
            MessageSent = DateTime.UtcNow;
        }
    }
}
