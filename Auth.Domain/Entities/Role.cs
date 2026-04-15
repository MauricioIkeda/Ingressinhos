using Generic.Domain.Entities;

namespace Auth.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Role(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do perfil");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new Exception("Deve ser informada a descricao do perfil");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Description = description.Trim();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do perfil");
        }

        Name = name.Trim();
    }

    public void ChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new Exception("Deve ser informada a descricao do perfil");
        }

        Description = description.Trim();
    }
}