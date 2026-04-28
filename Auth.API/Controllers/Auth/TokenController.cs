using Auth.API.Extensions;
using Auth.Application.Utils.Interface;
using Auth.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Auth.Application.Utils.Services;
using Generic.Application.Crud.Interface;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class TokenController : ControllerBase
{
    private readonly IToken _token;
    private readonly IUseCaseUserAuthColletion _userAuthUseCase; // preciso fazer
    public TokenController(IToken token, IUseCaseUserAuthColletion userAuth)
    {
        _token = token;
        _userAuth = userAuth;
    }

    [HttpPost("token")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest request)
    {
        try
        {
            if (request is null)
            {
                return BadRequest(new { message = "Deve ser informado o corpo da requisição." });
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email e senha devem ser informados." });
            }

            var email = new Email(request.Email);
            var user = _userAuthUseCase.Query(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            if (!PasswordHash.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            var token = _token.Generate(user);
            return Ok(new AuthenticateResponse(token));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public record AuthenticateRequest(string Email, string Password);
    public record AuthenticateResponse(string Token);
}
