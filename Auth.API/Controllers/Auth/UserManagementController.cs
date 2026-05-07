using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Generic.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public class UserManagementController : ControllerBase
{
    private readonly IUseCaseCreateUserAuth _createUserAuth;
    private readonly IUseCaseChangeUserEmail _changeUserEmail;
    private readonly IUseCaseDesactiveUser _deactivateUser;
    private readonly IUseCaseActivateUser _activateUser;

    public UserManagementController(
        IUseCaseCreateUserAuth createUserAuth,
        IUseCaseChangeUserEmail changeUserEmail,
        IUseCaseDesactiveUser deactivateUser,
        IUseCaseActivateUser activateUser)
    {
        _createUserAuth = createUserAuth;
        _changeUserEmail = changeUserEmail;
        _deactivateUser = deactivateUser;
        _activateUser = activateUser;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var result = _createUserAuth.Execute(new CreateUserRequest(request?.Name, request?.Email, request?.Password, request?.Role ?? 0));

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, new CreateUserResponse(result.Data));
    }

    [HttpPut("{userId}/email")]  // preciso mudar
    public IActionResult ChangeEmail(string userId, [FromBody] ChangeEmailRequest request)
    {
        var result = _changeUserEmail.Execute(new ChangeUserEmailDto(userId, request?.NewEmail));

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, new ChangeEmailResponse(true));
    }

    [HttpDelete("{userId}")] // Desativar usuário, não excluir
    public IActionResult DeactivateUser(string userId)
    {
        var result = _deactivateUser.Execute(userId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }
        return StatusCode(result.StatusCode);
    }

    [HttpPut("{userId}/activate")]
    public IActionResult ActivateUser(string userId)
    {
        var result = _activateUser.Execute(userId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }
}

