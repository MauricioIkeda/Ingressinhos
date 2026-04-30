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

    [HttpGet("{userId}/access")]  // preciso mudar
    public IActionResult GetAccess(string userId)
    {
        var result = _useCaseUserAccessQuery.Execute(userId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
