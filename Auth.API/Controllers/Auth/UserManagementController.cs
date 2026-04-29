using Auth.Domain.Entities;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Services;
using Auth.Domain.Enums;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public class UserManagementController : ControllerBase
{
    private readonly IUseCaseUserAuthCollection _userAuthUseCases;

    public UserManagementController(IUseCaseUserAuthCollection userAuthUseCases)
    {
        _userAuthUseCases = userAuthUseCases;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (request is null)
            {
                return BadRequest(new { message = "Deve ser informado o corpo da requisição." });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Deve ser informado o nome." });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Deve ser informado o email." });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Deve ser informada a senha." });
            }

            if (!Enum.IsDefined(typeof(RoleUser), request.Role))
            {
                return BadRequest(new { message = "Perfil (role) inválido." });
            }

            var email = new Email(request.Email);

            var emailInUse = _userAuthUseCases.Query(u => u.Email == email).Any();
            if (emailInUse)
            {
                return BadRequest(new { message = "Já existe um usuário cadastrado com esse email." });
            }

            var passwordHash = PasswordHash.Hash(request.Password);
            var user = new UserAuth(request.Name, request.Email, (RoleUser)request.Role, passwordHash);

            _userAuthUseCases.Include(user);

            return Ok(new CreateUserResponse(user.UserId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}/email")]
    public async Task<IActionResult> ChangeEmail(string userId, [FromBody] ChangeEmailRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { message = "Deve ser informado o identificador do usuário." });
            }

            if (request is null || string.IsNullOrWhiteSpace(request.NewEmail))
            {
                return BadRequest(new { message = "Deve ser informado o novo email." });
            }

            var user = _userAuthUseCases.Query(u => u.UserId == userId && u.Active).FirstOrDefault();
            if (user is null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            user.ChangeEmail(request.NewEmail);

            _userAuthUseCases.Update(user);

            return Ok(new ChangeEmailResponse(true));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public record CreateUserRequest(string Name, string Email, string Password, int Role);
    public record CreateUserResponse(string UserId);
    public record ChangeEmailRequest(string NewEmail);
    public record ChangeEmailResponse(bool Updated);
}
