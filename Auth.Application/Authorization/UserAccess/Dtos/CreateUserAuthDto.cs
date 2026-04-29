namespace Auth.Application.Authorization.UserAccess.Dtos;

public record CreateUserAuthDto(string Name, string Email, string Password, int Role);
