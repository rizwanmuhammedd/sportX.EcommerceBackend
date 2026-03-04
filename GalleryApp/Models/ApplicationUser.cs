using Microsoft.AspNetCore.Identity;

namespace GalleryApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}