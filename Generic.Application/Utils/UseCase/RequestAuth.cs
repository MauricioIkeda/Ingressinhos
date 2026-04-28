using System.Net.Http;
using System.Net.Http.Json;
using Generic.Application.Utils.Interface;

namespace Generic.Application.Utils.UseCase;

public sealed class RequestAuth : IRequestAuth
{
    private readonly HttpClient _httpClient;

    public RequestAuth(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> CreateUser(string name, string email, string password, int role)
    {
        var request = new CreateUserRequest
        {
            Name = name,
            Email = email,
            Password = password,
            Role = role
        };

        using var response = await _httpClient.PostAsJsonAsync("api/auth/users", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await GetErrorAsync(response, "Falha ao criar usuário no serviço de autenticação."));
        }

        var payload = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.UserId))
        {
            throw new Exception("Serviço de autenticação retornou uma resposta inválida ao criar usuário.");
        }

        return payload.UserId;
    }

    public async Task<string> Authenticate(string email, string password)
    {
        var request = new AuthenticateRequest
        {
            Email = email,
            Password = password
        };

        using var response = await _httpClient.PostAsJsonAsync("api/auth/token", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await GetErrorAsync(response, "Falha ao autenticar no serviço de autenticação."));
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
        {
            throw new Exception("Serviço de autenticação retornou uma resposta inválida ao autenticar.");
        }

        return payload.Token;
    }

    public async Task<bool> ChangeEmail(string userId, string newEmail)
    {
        try
        {
            var request = new ChangeEmailRequest
            {
                NewEmail = newEmail
            };

            var safeUserId = Uri.EscapeDataString(userId ?? string.Empty);
            using var response = await _httpClient.PutAsJsonAsync($"api/auth/users/{safeUserId}/email", request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var payload = await response.Content.ReadFromJsonAsync<ChangeEmailResponse>();
            return payload?.Updated ?? true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string> GetErrorAsync(HttpResponseMessage response, string fallback)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (!string.IsNullOrWhiteSpace(payload?.Message))
            {
                return payload.Message;
            }
        }
        catch
        {
            // ignore
        }

        return $"{fallback} Status {(int)response.StatusCode} ({response.ReasonPhrase}).";
    }

    private sealed class CreateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }

    private sealed class CreateUserResponse
    {
        public string UserId { get; set; }
    }

    private sealed class AuthenticateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    private sealed class AuthenticateResponse
    {
        public string Token { get; set; }
    }

    private sealed class ChangeEmailRequest
    {
        public string NewEmail { get; set; }
    }

    private sealed class ChangeEmailResponse
    {
        public bool Updated { get; set; }
    }

    private sealed class ErrorResponse
    {
        public string Message { get; set; }
    }
}
