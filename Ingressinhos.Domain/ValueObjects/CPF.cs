namespace Ingressinhos.Domain.ValueObjects;

public record CPF
{
    public string Numero { get; init; }

    public CPF(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            throw new Exception("O CPF eh obrigatorio e nao pode ser vazio");
        }
        
        var numeroTratado = Limpar(numero);

        if (!Validar(numeroTratado))
        {
            throw new Exception("CPF Invalido");
        }
        
        Numero = numeroTratado;
    }

    private static string Limpar(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }
    
    private static bool Validar(string cpf)
    {
        if (cpf.Length != 11) 
            return false;
        
        if (cpf.Distinct().Count() == 1) 
            return false;

        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        if (resto < 2) 
            resto = 0; 
        else 
            resto = 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2) 
            resto = 0; 
        else 
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }

    public string Formatado()
    {
        return Convert.ToUInt64(Numero).ToString(@"000\.000\.000\-00");
    }

    public override string ToString()
    {
        return Numero;
    }
}