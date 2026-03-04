using System;

namespace GalleryApp.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ViewCount { get; set; } = 0;
        public string? Category { get; set; }
        public string? UserId { get; set; }
        public string? UploaderEmail { get; set; }
    }
}