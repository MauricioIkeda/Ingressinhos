using Generic.Domain.Entities;
using System.Text.RegularExpressions;

namespace Generic.Domain.ValueObjects;

public class Email : ValidatableObject
{
    public string Endereco { get; init; } = string.Empty;

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            AddError("E-mail", "Informe um e-mail.");
            return;
        }
        
        var emailFormatado = endereco.Trim().ToLowerInvariant();

        if (!Validar(emailFormatado))
        {
            AddError("E-mail", "Informe um e-mail valido.");
            return;
        }
        
        Endereco = emailFormatado;
    }

    private static bool Validar(string endereco)
    {
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(endereco);
    }

    public override string ToString()
    {
        return Endereco;
    }
}
