namespace Ingressinhos.Application.Sales.Dtos;

public class ClientDto
{
    public long ClientId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string Password { get; set; } // somente para inclusão
}