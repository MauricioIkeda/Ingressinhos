using Generic.Domain.ValueObjects;

namespace Generic.Domain.Entities;

public abstract class User : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = new(string.Empty);
    public bool Active { get; private set; } = true;
    
    protected User() { }

    protected User(string name, string email, string userId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddError("Nome", "Informe o nome.");
        }
        else
        {
            Name = name.Trim();
        }

        var emailValue = new Email(email);
        CopyErrorsFrom(emailValue);
        if (emailValue.IsValid)
        {
            Email = emailValue;
        }

        UserId = userId;
    }

    public void AttachUserId(string userId)
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(userId))
        {
            AddError("Conta", "Nao foi possivel vincular a conta do usuario.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(UserId))
        {
            AddError("Conta", "A conta do usuario ja foi vinculada.");
            return;
        }

        UserId = userId.Trim();
    }

    public void ChangeName(string name)
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(name))
        {
            AddError("Nome", "Informe o nome.");
            return;
        }

        Name = name.Trim();
    }

    public void ChangeEmail(string email)
    {
        ClearErrors();

        var emailValue = new Email(email);
        CopyErrorsFrom(emailValue);
        if (!emailValue.IsValid)
        {
            return;
        }

        Email = emailValue;
    }

    public void Deactivate()
    {
        ClearErrors();
        Active = false;
    }

    public void Activate()
    {
        ClearErrors();
        Active = true;
    }
}
