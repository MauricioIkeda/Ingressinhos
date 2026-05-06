using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpGet("access")]  // preciso mudar
    public IActionResult GetAccess()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        OperationResult<UserAccessDto> result = _useCaseUserAccessQuery.Execute(userId);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
