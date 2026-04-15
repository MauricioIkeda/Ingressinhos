using Generic.Domain.ValueObjects;

namespace Generic.Domain.Entities;

public abstract class User : BaseEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public CPF Cpf { get; private set; }

    protected User(string name, string email, string cpf)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do usuario");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = new Email(email);
        Cpf = new CPF(cpf);
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do usuario");
        }

        Name = name.Trim();
    }

    public void ChangeEmail(string email)
    {
        Email = new Email(email);
    }
}