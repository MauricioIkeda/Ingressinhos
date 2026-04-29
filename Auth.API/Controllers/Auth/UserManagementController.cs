using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public class UserManagementController : ControllerBase
{
    private readonly IRepositorySession _repositorySession;

    public UserManagementController(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (request is null)
        {
            return StatusCode(422, [new MensagemErro("Body", "Deve ser informado o corpo da requisicao.")]);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return StatusCode(422, [new MensagemErro("Name", "Deve ser informado o nome.")]);
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return StatusCode(422, [new MensagemErro("Email", "Deve ser informado o email.")]);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return StatusCode(422, [new MensagemErro("Password", "Deve ser informada a senha.")]);
        }

        if (!Enum.IsDefined(typeof(RoleUser), request.Role))
        {
            return StatusCode(422, [new MensagemErro("Role", "Perfil (role) invalido.")]);
        }

        try
        {
            var email = new Email(request.Email);
            var repositoryQuery = _repositorySession.GetRepositoryQuery();

            var emailInUse = repositoryQuery.Query<UserAuth>(u => u.Email == email).Any();
            if (emailInUse)
            {
                return StatusCode(400, [new MensagemErro("Email", "Ja existe um usuario cadastrado com esse email.")]);
            }

            var passwordHash = PasswordHash.Hash(request.Password);
            var user = new UserAuth(request.Name, request.Email, (RoleUser)request.Role, passwordHash);

            var repository = _repositorySession.GetRepository();
            repository.Include(user);
            await repository.Flush();

            return StatusCode(201, new CreateUserResponse(user.UserId));
        }
        catch (Exception ex)
        {
            return StatusCode(422, [MensagemErro.Geral(ex.Message)]);
        }
    }

    [HttpPut("{userId}/email")]
    public async Task<IActionResult> ChangeEmail(string userId, [FromBody] ChangeEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return StatusCode(422, [new MensagemErro("UserId", "Deve ser informado o identificador do usuario.")]);
        }

        if (request is null || string.IsNullOrWhiteSpace(request.NewEmail))
        {
            return StatusCode(422, [new MensagemErro("NewEmail", "Deve ser informado o novo email.")]);
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var user = repositoryQuery.Query<UserAuth>(u => u.UserId == userId && u.Active).FirstOrDefault();
            if (user is null)
            {
                return StatusCode(404, [new MensagemErro("UserId", "Usuario nao encontrado.")]);
            }

            user.ChangeEmail(request.NewEmail);

            var repository = _repositorySession.GetRepository();
            repository.Upsert(user);
            await repository.Flush();

            return StatusCode(200, new ChangeEmailResponse(true));
        }
        catch (Exception ex)
        {
            return StatusCode(422, [MensagemErro.Geral(ex.Message)]);
        }
    }

    public record CreateUserRequest(string Name, string Email, string Password, int Role);
    public record CreateUserResponse(string UserId);
    public record ChangeEmailRequest(string NewEmail);
    public record ChangeEmailResponse(bool Updated);
}
