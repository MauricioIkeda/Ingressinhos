namespace Generic.Api.Dtos;

public sealed record ResultOData<T>(long Count, List<T> Data);
