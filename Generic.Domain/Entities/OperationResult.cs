namespace Generic.Domain.Entities;

public class MensagemErro(string propriedade, string mensagem)
{
    public string Propriedade { get; set; } = propriedade;
    public string Mensagem { get; set; } = mensagem;

    public MensagemErro(string mensagem) : this(string.Empty, mensagem)
    {
    }

    public static MensagemErro Geral(string mensagem)
    {
        return new(string.Empty, mensagem);
    }
}

public class OperationResult
{
    private readonly List<MensagemErro> _errors = [];

    public bool Success { get; protected init; }
    public int StatusCode { get; protected init; }
    public IReadOnlyList<MensagemErro> Errors => _errors;

    protected OperationResult(bool success, int statusCode, IEnumerable<MensagemErro> errors = null)
    {
        Success = success;
        StatusCode = statusCode;

        if (errors is null)
        {
            return;
        }

        _errors.AddRange(errors.Where(e => e is not null));
    }

    public static OperationResult Ok() => new(true, 200);
    public static OperationResult Created() => new(true, 201);
    public static OperationResult Fail(IEnumerable<MensagemErro> errors) => new(false, 400, errors);
    public static OperationResult Fail(MensagemErro error) => new(false, 400, [error]);
    public static OperationResult NotFound(MensagemErro error) => new(false, 404, [error]);
    public static OperationResult Unauthorized(MensagemErro error) => new(false, 401, [error]);
    public static OperationResult Forbidden(MensagemErro error) => new(false, 403, [error]);
    public static OperationResult UnprocessableEntity(IEnumerable<MensagemErro> errors) => new(false, 422, errors);
    public static OperationResult UnprocessableEntity(MensagemErro error) => new(false, 422, [error]);
    public static OperationResult FatalError(MensagemErro error) => new(false, 500, [error]);
}

public class OperationResult<T> : OperationResult
{
    public T Data { get; private init; }

    private OperationResult(bool success, int statusCode, T data = default, IEnumerable<MensagemErro> errors = null)
        : base(success, statusCode, errors)
    {
        Data = data;
    }

    public static OperationResult<T> Ok(T data) => new(true, 200, data);
    public static OperationResult<T> Created(T data) => new(true, 201, data);
    public static new OperationResult<T> Created() => new(true, 201);
    public static new OperationResult<T> Fail(IEnumerable<MensagemErro> errors) => new(false, 400, default, errors);
    public static new OperationResult<T> Fail(MensagemErro error) => new(false, 400, default, [error]);
    public static new OperationResult<T> NotFound(MensagemErro error) => new(false, 404, default, [error]);
    public static new OperationResult<T> Unauthorized(MensagemErro error) => new(false, 401, default, [error]);
    public static new OperationResult<T> Forbidden(MensagemErro error) => new(false, 403, default, [error]);
    public static new OperationResult<T> UnprocessableEntity(IEnumerable<MensagemErro> errors) => new(false, 422, default, errors);
    public static new OperationResult<T> UnprocessableEntity(MensagemErro error) => new(false, 422, default, [error]);
    public static new OperationResult<T> FatalError(MensagemErro error) => new(false, 500, default, [error]);
    public static OperationResult<T> FromResult(OperationResult result) => new(result.Success, result.StatusCode, default, result.Errors);
}
