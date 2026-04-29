using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public class UserManagementController : ControllerBase
{
    private readonly IUseCaseCreateUserAuth _createUserAuth;
    private readonly IUseCaseChangeUserEmail _changeUserEmail;

    public UserManagementController(IUseCaseCreateUserAuth createUserAuth, IUseCaseChangeUserEmail changeUserEmail)
    {
        _createUserAuth = createUserAuth;
        _changeUserEmail = changeUserEmail;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var result = _createUserAuth.Execute(new CreateUserAuthDto(request?.Name, request?.Email, request?.Password, request?.Role ?? 0));

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

    //Dtos somente usados aqui
    public record CreateUserRequest(string Name, string Email, string Password, int Role);
    public record CreateUserResponse(string UserId);
    public record ChangeEmailRequest(string NewEmail);
    public record ChangeEmailResponse(bool Updated);
}
