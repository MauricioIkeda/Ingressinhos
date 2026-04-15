using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Sales.Entities;

public class Client : User
{
    public Client(string name, string email, string cpf) : base(name, email, cpf)
    {
    }
}