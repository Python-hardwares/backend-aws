using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TB2_aws.Data;
using TB2_aws.DTOs;
using TB2_aws.Models;
using TB2_aws.Service;

namespace TB2_aws.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext context,
    IPasswordHasher<Usuario> passwordHasher,
    JwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDTO>> Login(
        LoginDTO login,
        CancellationToken cancellationToken)
    {
        var email = login.Email.Trim().ToLowerInvariant();
        var usuario = await context.Usuarios
            .SingleOrDefaultAsync(item => item.Email == email, cancellationToken);

        if (usuario is null ||
            passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, login.Senha)
            == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        var (token, expiresAt) = jwtService.GenerateToken(usuario);
        return Ok(new AuthResponseDTO
        {
            Token = token,
            ExpiraEm = expiresAt,
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        });
    }
}
