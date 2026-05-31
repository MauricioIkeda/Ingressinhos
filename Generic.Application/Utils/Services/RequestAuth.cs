using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;

namespace Generic.Application.Utils.Services;

public sealed class RequestAuth : IRequestAuth
{
    private readonly HttpClient _httpClient; // usar using para dar dispose automaticamente e não esquecermos
    private readonly SentinelAuthClientOptions _options;

    public RequestAuth(HttpClient httpClient, SentinelAuthClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<OperationResult<string>> CreateUser(string name, string email, string password, int role)
    {
        var request = new SentinelRegisterUserRequest(name, email, password);
        const string propertyName = "Conta";
        var roleId = _options.GetRoleId(role);
        if (_options.ApplicationClientId <= 0 || roleId <= 0)
        {
            return OperationResult<string>.FatalError(new MensagemErro(propertyName, "Configuracao do SentinelAuth esta incompleta."));
        }

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/User/register", request);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<string>(response, propertyName, "Nao foi possivel criar a conta agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<SentinelRegisterUserResponse>();
            if (payload is null || payload.Id <= 0)
            {
                return OperationResult<string>.FatalError(new MensagemErro(propertyName, "Nao conseguimos concluir a criacao da conta agora."));
            }

            var assignRoleRequest = new SentinelAssignRoleRequest(payload.Id, _options.ApplicationClientId, roleId);
            using var assignRoleResponse = await _httpClient.PostAsJsonAsync("api/user-roles/assign", assignRoleRequest);

            if (!assignRoleResponse.IsSuccessStatusCode)
            {
                await DeactivateUser(payload.Id.ToString());
                return await CreateFailureResultAsync<string>(assignRoleResponse, propertyName, "Nao foi possivel vincular a permissao da conta agora.");
            }

            return OperationResult<string>.Created(payload.Id.ToString());
        }
        catch
        {
            return OperationResult<string>.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de conta agora."));
        }
    }

    public async Task<OperationResult> ChangeEmail(string userId, string newEmail)
    {
        const string propertyName = "E-mail";

        try
        {
            var request = new ChangeEmailRequest(newEmail);

            var safeUserId = Uri.EscapeDataString(userId ?? string.Empty);
            using var response = await _httpClient.PutAsJsonAsync($"api/User/{safeUserId}/email", request);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync(response, propertyName, "Nao foi possivel atualizar o e-mail agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<ChangeEmailResponse>();
            if (payload is null || !payload.Updated)
            {
                return OperationResult.FatalError(new MensagemErro(propertyName, "Nao conseguimos atualizar o e-mail agora."));
            }

            return OperationResult.Ok();
        }
        catch
        {
            return OperationResult.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de conta agora."));
        }
    }

    public async Task<OperationResult> DeactivateUser(string userId)
    {
        const string propertyName = "Conta";

        try
        {
            var safeUserId = Uri.EscapeDataString(userId ?? string.Empty);
            using var response = await _httpClient.DeleteAsync($"api/User/{safeUserId}");

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync(response, propertyName, "Nao foi possivel desativar a conta agora.");
            }

            return OperationResult.Ok();
        }
        catch
        {
            return OperationResult.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de conta agora."));
        }
    }

    public async Task<OperationResult> ActivateUser(string userId)
    {
        const string propertyName = "Conta";

        try
        {
            var safeUserId = Uri.EscapeDataString(userId ?? string.Empty);
            using var response = await _httpClient.PutAsync($"api/User/{safeUserId}/activate", content: null);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync(response, propertyName, "Nao foi possivel recuperar a conta agora.");
            }

            return OperationResult.Ok();
        }
        catch
        {
            return OperationResult.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de conta agora."));
        }
    }

    public async Task<OperationResult> AssignRole(string userId, int role)
    {
        const string propertyName = "Perfil";
        var roleId = _options.GetRoleId(role);
        if (_options.ApplicationClientId <= 0 || roleId <= 0 || !long.TryParse(userId, out var parsedUserId))
        {
            return OperationResult.FatalError(new MensagemErro(propertyName, "Configuracao do SentinelAuth esta incompleta."));
        }

        try
        {
            var request = new SentinelAssignRoleRequest(parsedUserId, _options.ApplicationClientId, roleId);
            using var response = await _httpClient.PostAsJsonAsync("api/user-roles/assign", request);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync(response, propertyName, "Nao foi possivel vincular a permissao da conta agora.");
            }

            return OperationResult.Ok();
        }
        catch
        {
            return OperationResult.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de conta agora."));
        }
    }

    private static async Task<OperationResult> CreateFailureResultAsync(HttpResponseMessage response, string propertyName, string fallback)
    {
        var errors = await GetErrorsAsync(response, propertyName, fallback);
        var firstError = errors.FirstOrDefault() ?? new MensagemErro(propertyName, fallback);

        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => OperationResult.Fail(errors),
            HttpStatusCode.Unauthorized => OperationResult.Unauthorized(firstError),
            HttpStatusCode.Forbidden => OperationResult.Forbidden(firstError),
            HttpStatusCode.NotFound => OperationResult.NotFound(firstError),
            HttpStatusCode.UnprocessableEntity => OperationResult.UnprocessableEntity(errors),
            _ => OperationResult.FatalError(firstError)
        };
    }

    private static async Task<OperationResult<T>> CreateFailureResultAsync<T>(HttpResponseMessage response, string propertyName, string fallback)
    {
        return OperationResult<T>.FromResult(await CreateFailureResultAsync(response, propertyName, fallback));
    }

    private static async Task<List<MensagemErro>> GetErrorsAsync(HttpResponseMessage response, string propertyName, string fallback)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<MensagemErro> { new MensagemErro(propertyName, fallback) };
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var errors = TryDeserialize<List<MensagemErro>>(content, jsonOptions);
            if (errors is { Count: > 0 })
            {
                return errors;
            }

            var problem = TryDeserialize<ErrorResponse>(content, jsonOptions);
            if (!string.IsNullOrWhiteSpace(problem?.Message))
            {
                return new List<MensagemErro> { new MensagemErro(propertyName, problem.Message) };
            }

            var problemDetails = TryDeserialize<ProblemDetailsResponse>(content, jsonOptions);
            if (!string.IsNullOrWhiteSpace(problemDetails?.Detail))
            {
                return new List<MensagemErro> { new MensagemErro(propertyName, problemDetails.Detail) };
            }
        }
        catch
        {
        }

        return new List<MensagemErro> { new MensagemErro(propertyName, fallback) };
    }

    private static T TryDeserialize<T>(string content, JsonSerializerOptions options)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(content, options);
        }
        catch
        {
            return default;
        }
    }
}
