using System.Net;
using System.Net.Http.Json;
using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;

namespace Generic.Application.Utils.Services;

public sealed class RequestAuth : IRequestAuth
{
    private readonly HttpClient _httpClient; // usar using para dar dispose automaticamente e não esquecermos

    public RequestAuth(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<OperationResult<string>> CreateUser(string name, string email, string password, int role)
    {
        var request = new CreateUserRequest(name, email, password, role);
        const string propertyName = "Conta";

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/auth/users", request);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<string>(response, propertyName, "Nao foi possivel criar a conta agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
            if (payload is null || string.IsNullOrWhiteSpace(payload.UserId))
            {
                return OperationResult<string>.FatalError(new MensagemErro(propertyName, "Nao conseguimos concluir a criacao da conta agora."));
            }

            return OperationResult<string>.Created(payload.UserId);
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
            using var response = await _httpClient.PutAsJsonAsync($"api/auth/users/{safeUserId}/email", request);

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
            var errors = await response.Content.ReadFromJsonAsync<List<MensagemErro>>();
            if (errors is { Count: > 0 })
            {
                return errors;
            }
        }
        catch
        {
        }

        return new List<MensagemErro> { new MensagemErro(propertyName, fallback) };
    }
}
