using Microsoft.AspNetCore.Http;

public class UploadAvatarDto
{
    public IFormFile Avatar { get; set; } = null!;
}
