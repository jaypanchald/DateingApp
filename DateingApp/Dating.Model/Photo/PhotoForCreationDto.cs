using Microsoft.AspNetCore.Http;
using System;

namespace Dating.Model.Photo
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public string PublicId { get; set; }
    }
}
