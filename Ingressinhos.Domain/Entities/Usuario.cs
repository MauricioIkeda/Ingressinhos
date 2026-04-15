using Generic.Domain.Entities;
using Ingressinhos.Domain.ValueObjects;

namespace Ingressinhos.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public string Senha { get; private set; }
    public CPF Cpf { get; private set; }

    protected Usuario(string  nome, string email, string senha, string cpf)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = new Email(email);
        Senha = senha;
        Cpf = new CPF(cpf);
    }
}