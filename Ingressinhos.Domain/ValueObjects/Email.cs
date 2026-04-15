using System.Text.RegularExpressions;

namespace Ingressinhos.Domain.ValueObjects;

public record Email
{
    public string Endereco { get; init; }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            throw new Exception("Deve ser informado o email");
        }
        
        var emailFormatado = endereco.Trim().ToLower();

        if (!Validar(emailFormatado))
        {
            throw new Exception("O email deve ser valido!");
        }
        
        Endereco = endereco;
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