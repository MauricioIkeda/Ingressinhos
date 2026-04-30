namespace Generic.Application.Utils.Interface;

public interface ICurrentUserContext  // Interface para obter informações do usuário autenticado
{
    bool IsAuthenticated { get; }
    string UserId { get; }
    string Role { get; }
}
