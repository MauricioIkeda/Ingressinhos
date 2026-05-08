using System.Net;
using System.Net.Http.Json;
using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;

namespace Generic.Application.Utils.Services;

public sealed class RequestPayment : IRequestPayment
{
    private readonly HttpClient _httpClient;

    public RequestPayment(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<OperationResult<PaymentTransactionApiDto>> CreatePayment(long orderId, decimal amount, string method)
    {
        const string propertyName = "Pagamento";

        try
        {
            var request = new RequestPaymentApiDto
            {
                OrderId = orderId,
                Amount = amount,
                Method = method
            };

            using var response = await _httpClient.PostAsJsonAsync("api/payments/transactions", request);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<PaymentTransactionApiDto>(response, propertyName, "Nao foi possivel solicitar o pagamento agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<PaymentTransactionApiDto>();
            if (payload is null)
            {
                return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos concluir a solicitacao do pagamento agora."));
            }

            return OperationResult<PaymentTransactionApiDto>.Created(payload);
        }
        catch
        {
            return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de pagamento agora."));
        }
    }

    public async Task<OperationResult<PaymentTransactionApiDto>> GetPaymentById(long paymentTransactionId)
    {
        const string propertyName = "Pagamento";

        try
        {
            using var response = await _httpClient.GetAsync($"api/payments/transactions/{paymentTransactionId}");

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<PaymentTransactionApiDto>(response, propertyName, "Nao foi possivel consultar o pagamento agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<PaymentTransactionApiDto>();
            if (payload is null)
            {
                return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos carregar o pagamento agora."));
            }

            return OperationResult<PaymentTransactionApiDto>.Ok(payload);
        }
        catch
        {
            return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de pagamento agora."));
        }
    }

    public async Task<OperationResult<List<PaymentTransactionApiDto>>> GetPaymentsByOrder(long orderId)
    {
        const string propertyName = "Pagamento";

        try
        {
            using var response = await _httpClient.GetAsync($"api/payments/transactions/order/{orderId}");

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<List<PaymentTransactionApiDto>>(response, propertyName, "Nao foi possivel consultar os pagamentos do pedido agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<List<PaymentTransactionApiDto>>();
            if (payload is null)
            {
                return OperationResult<List<PaymentTransactionApiDto>>.FatalError(new MensagemErro(propertyName, "Nao conseguimos carregar os pagamentos do pedido agora."));
            }

            return OperationResult<List<PaymentTransactionApiDto>>.Ok(payload);
        }
        catch
        {
            return OperationResult<List<PaymentTransactionApiDto>>.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de pagamento agora."));
        }
    }

    public async Task<OperationResult<PaymentTransactionApiDto>> CheckPaymentStatus(long orderId)
    {
        const string propertyName = "Pagamento";

        try
        {
            using var response = await _httpClient.PostAsync($"api/payments/transactions/order/{orderId}/status/check", content: null);

            if (!response.IsSuccessStatusCode)
            {
                return await CreateFailureResultAsync<PaymentTransactionApiDto>(response, propertyName, "Nao foi possivel verificar o status do pagamento agora.");
            }

            var payload = await response.Content.ReadFromJsonAsync<PaymentTransactionApiDto>();
            if (payload is null)
            {
                return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos atualizar o status do pagamento agora."));
            }

            return OperationResult<PaymentTransactionApiDto>.Ok(payload);
        }
        catch
        {
            return OperationResult<PaymentTransactionApiDto>.FatalError(new MensagemErro(propertyName, "Nao conseguimos falar com o servico de pagamento agora."));
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

        return [new MensagemErro(propertyName, fallback)];
    }
}
