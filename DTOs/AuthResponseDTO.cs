namespace TB2_aws.DTOs;

public class AuthResponseDTO
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEm { get; init; }
    public int UsuarioId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
