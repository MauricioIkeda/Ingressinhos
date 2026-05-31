

namespace Generic.Application.Dtos
{
    public record CreateUserRequest(string Name, string Email, string Password, int Role);
    public record CreateUserResponse(string UserId);
    public record SentinelRegisterUserRequest(string Name, string Email, string Password);
    public record SentinelRegisterUserResponse(long Id, string Username, string Password);
    public record SentinelAssignRoleRequest(long UserId, long ApplicationClientId, long RoleId);
    public record AuthenticateRequest(string Email, string Password);
    public record RefreshTokenRequest(string Token, string RefreshToken);
    public record AuthenticateResponse(string Token, string RefreshToken);
    public record ChangeEmailRequest(string NewEmail);
    public record ChangeEmailResponse(bool Updated);
    public record ErrorResponse(string Message);
    public record ProblemDetailsResponse(string Title, string Detail);
}
