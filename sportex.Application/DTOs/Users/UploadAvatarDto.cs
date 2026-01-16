using Microsoft.AspNetCore.Http;

namespace Sportex.Application.DTOs.Users;

public class UploadAvatarDto
{
    public IFormFile Image { get; set; } = null!;
}
