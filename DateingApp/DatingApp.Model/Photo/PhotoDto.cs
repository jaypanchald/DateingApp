using System;

namespace DatingApp.Model.Photo
{
    public class PhotoDto
    {
        public string Url { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public bool IsMain { get; set; }
    }
}
