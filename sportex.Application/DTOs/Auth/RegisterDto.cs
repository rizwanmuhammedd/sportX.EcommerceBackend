using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Auth;

public class RegisterDto
{
    private string _name = null!;

    //[DefaultValue("yourName")]
    //[Required(ErrorMessage = "Name is required")]
    //[MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    //[MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    //[RegularExpression(
    //    @"^[A-Z][a-z]{2,49}$",
    //    ErrorMessage = "Name must be a single word, start with capital letter, and contain only letters"
    //)]
    //public string Name
    //{
    //    get => _name;
    //    set => _name = value?.Trim() ?? string.Empty;
    //}



    [DefaultValue("yourName")]
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    [RegularExpression(
@"^[A-Za-z]{3,50}$",
ErrorMessage = "Name must contain only letters, no spaces, and at least 3 characters"
)]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;   
    }



    [DefaultValue("yourEmail")]
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    [RegularExpression(
@"^(?!example@)(?!test@)(?!demo@)(?!admin@)(?!user@)[a-z0-9]+([._%+-][a-z0-9]+)*@[a-z0-9-]+\.(com|in|net|org|co|edu)$",
ErrorMessage = "Email must be a valid real domain (.com, .in, .net, .org, .co, .edu)"
)]
    public string Email { get; set; } = null!;

    [DefaultValue("yourPass")]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
    [RegularExpression(
        @"^(?=(?:.*[A-Z]))(?=(?:.*[a-z]))(?=(?:.*\d))(?=(?:.*[@$!%*?&]))(?!.*\s)(?!.*(.)\1\1).{8,50}$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, special character and no spaces"
    )]
    public string Password { get; set; } = null!;
}
