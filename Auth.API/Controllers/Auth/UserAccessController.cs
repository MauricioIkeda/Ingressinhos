using Auth.Application.Authorization.UserAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public class UserAccessController : ControllerBase
{
    private readonly IUseCaseUserAccessQuery _useCaseUserAccessQuery;

    public UserAccessController(IUseCaseUserAccessQuery useCaseUserAccessQuery)
    {
        _useCaseUserAccessQuery = useCaseUserAccessQuery;
    }

    [HttpGet("{userId:long}/access")] // Precisa fazer uma forma bem mais complexa de pegar o userId, no momento basicamente não tem segurança
    public IActionResult GetAccess(long userId)
    {
        try
        {
            var result = _useCaseUserAccessQuery.Execute(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}