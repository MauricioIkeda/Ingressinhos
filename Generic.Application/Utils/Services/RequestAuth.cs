using System.Net.Http.Json;
using System.Text.Json;
using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;

namespace Generic.Application.Utils.Services;

public sealed class RequestAuth : IRequestAuth
{
    private readonly HttpClient _httpClient;

    public RequestAuth(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> CreateUser(string name, string email, string password, int role)
    {
        var request = new CreateUserRequest(name, email, password, role);

        using var response = await _httpClient.PostAsJsonAsync("api/auth/users", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await GetErrorAsync(response, "Falha ao criar usuario no servico de autenticacao."));
        }

        var payload = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.UserId))
        {
            throw new Exception("Servico de autenticacao retornou uma resposta invalida ao criar usuario.");
        }

        return payload.UserId;
    }

    public async Task<string> Authenticate(string email, string password)
    {
        var request = new AuthenticateRequest(email, password);

        using var response = await _httpClient.PostAsJsonAsync("api/auth/token", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await GetErrorAsync(response, "Falha ao autenticar no servico de autenticacao."));
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
        {
            throw new Exception("Servico de autenticacao retornou uma resposta invalida ao autenticar.");
        }

        return payload.Token;
    }

    public async Task<bool> ChangeEmail(string userId, string newEmail)
    {
        try
        {
            var request = new ChangeEmailRequest(newEmail);

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
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return $"{fallback} Status {(int)response.StatusCode} ({response.ReasonPhrase}).";
            }

            if (TryExtractErrorMessage(content, out var message))
            {
                return message;
            }

            return content;
        }
        catch
        {
            // Se falhar ao ler a resposta, seguimos com a mensagem padrao.
        }

        return $"{fallback} | Status: {(int)response.StatusCode} | ({response.ReasonPhrase}).";
    }

    private static bool TryExtractErrorMessage(string content, out string message)
    {
        message = string.Empty;

        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    if (TryGetPropertyValue(item, "mensagem", out message) ||
                        TryGetPropertyValue(item, "message", out message))
                    {
                        return true;
                    }
                }
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (TryGetPropertyValue(root, "mensagem", out message) ||
                    TryGetPropertyValue(root, "message", out message) ||
                    TryGetPropertyValue(root, "error", out message) ||
                    TryGetPropertyValue(root, "title", out message) ||
                    TryGetPropertyValue(root, "detail", out message))
                {
                    return true;
                }
            }
        }
        catch (JsonException)
        {
            message = content.Trim();
            return !string.IsNullOrWhiteSpace(message);
        }

        return false;
    }

    private static bool TryGetPropertyValue(JsonElement element, string propertyName, out string value)
    {
        value = string.Empty;

        foreach (var property in element.EnumerateObject())
        {
            if (!string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.String)
            {
                value = property.Value.GetString() ?? string.Empty;
                return !string.IsNullOrWhiteSpace(value);
            }

            value = property.Value.ToString();
            return !string.IsNullOrWhiteSpace(value);
        }

        return false;
    }
}
