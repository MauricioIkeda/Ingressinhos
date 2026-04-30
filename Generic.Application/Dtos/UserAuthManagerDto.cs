

namespace Generic.Application.Dtos
{
    public record CreateUserRequest(string Name, string Email, string Password, int Role);

    public record CreateUserResponse(string UserId);

    public record AuthenticateRequest(string Email, string Password);

    public record AuthenticateResponse(string Token);
    public record ChangeEmailRequest(string NewEmail);

    public record ChangeEmailResponse(bool Updated);
    public record ErrorResponse(string Message);
}
