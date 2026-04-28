using Generic.Domain.ValueObjects;

namespace Generic.Domain.Entities;

public abstract class User : BaseEntity
{
    public string UserId { get; private set; }  // Id para identificação no Auth, nunca expor fora da aplicação
    public string Name { get; private set; }
    public Email Email { get; private set; }
    
    protected User() { }

    protected User(string name, string email, string userId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do usuario");
        }

        Name = name.Trim();
        Email = new Email(email);
        UserId = userId;
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