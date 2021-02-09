﻿using System;

namespace Dating.Model.Photo
{
    public class PhotoForReturnDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public bool IsMain { get; set; }

        public string PublicId { get; set; }
    }
}
