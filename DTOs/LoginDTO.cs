using System.ComponentModel.DataAnnotations;

namespace TB2_aws.DTOs;

public class LoginDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}
