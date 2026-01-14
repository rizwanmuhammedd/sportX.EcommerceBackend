using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
