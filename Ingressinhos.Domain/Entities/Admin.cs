namespace Ingressinhos.Domain.Entities;

public class Admin : Usuario
{
    protected Admin(string nome, string email, string senha, string cpf) : base(nome, email, senha, cpf)
    {
    }
}