using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Ingressinhos.Domain.Sales.Entities;

public class Client : User
{
    public CPF Cpf { get; private set; }

    public Client(string name, string email, string cpf) : base(name, email)
    {
        Cpf = new CPF(cpf);
    }
}