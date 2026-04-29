using Auth.API.Extensions;
using Auth.Application.Utils.Interface;
using Auth.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Auth.Application.Authorization.UserAccess.Interfaces;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class TokenController : ControllerBase
{
    private readonly IToken _token;
    private readonly IUseCaseUserAuthCollection _authUseCase; // preciso fazer
    public TokenController(IToken token, IUseCaseUserAuthCollection authUseCase)
    {
        _token = token;
        _authUseCase = authUseCase;
    }

    [HttpPost("login")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest request)
    {
        (bool success, string token) = _authUseCase.Execute(request.Email, request.Password);

        if (!success)
            return Unauthorized(_authUseCase.Messages);

        return Ok(new AuthenticateResponse(token));
    }

    public record AuthenticateRequest(string Email, string Password);
    public record AuthenticateResponse(string Token);
}
