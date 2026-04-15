using Ingressinhos.Domain.ValueObjects;

namespace Ingressinhos.Domain.Entities;

public class Cliente : Usuario
{
    protected Cliente(string nome, string email, string senha, string cpf) : base(nome, email, senha, cpf)
    {
    }
}