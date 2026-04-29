using Auth.Application.Authorization.UserAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class TokenController : ControllerBase
{
    private readonly IUseCaseUserAuthCollection _authUseCase;

    public TokenController(IUseCaseUserAuthCollection authUseCase)
    {
        _authUseCase = authUseCase;
    }

    [HttpPost("login")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest request)
    {
        var result = _authUseCase.Execute(request.Email, request.Password);

        if (!result.Success)
            return StatusCode(result.StatusCode, result.Errors);

        return StatusCode(result.StatusCode, new AuthenticateResponse(result.Data));
    }

    public record AuthenticateRequest(string Email, string Password);
    public record AuthenticateResponse(string Token);
}
