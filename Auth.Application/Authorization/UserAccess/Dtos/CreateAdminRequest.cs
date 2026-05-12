namespace Auth.Application.Authorization.UserAccess.Dtos;

public record CreateAdminRequest(string Name, string Email, string Password);
