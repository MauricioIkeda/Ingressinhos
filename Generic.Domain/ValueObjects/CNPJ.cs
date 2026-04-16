namespace Generic.Domain.ValueObjects;

public record CNPJ
{
	public string Numero { get; init; }

	public CNPJ(string numero)
	{
		if (string.IsNullOrWhiteSpace(numero))
		{
			throw new Exception("O CNPJ eh obrigatorio e nao pode ser vazio");
		}

		var numeroTratado = Limpar(numero);

		if (!Validar(numeroTratado))
		{
			throw new Exception("CNPJ Invalido");
		}

		Numero = numeroTratado;
	}

	private static string Limpar(string cnpj)
	{
		return new string(cnpj.Where(char.IsDigit).ToArray());
	}

	private static bool Validar(string cnpj)
	{
		if (cnpj.Length != 14)
			return false;

		if (cnpj.Distinct().Count() == 1)
			return false;

		int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
		string tempCnpj = cnpj.Substring(0, 12);
		int soma = 0;

		for (int i = 0; i < 12; i++)
			soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

		int resto = soma % 11;
		if (resto < 2)
			resto = 0;
		else
			resto = 11 - resto;

		string digito = resto.ToString();
		tempCnpj = tempCnpj + digito;
		soma = 0;

		int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
		for (int i = 0; i < 13; i++)
			soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

		resto = soma % 11;
		if (resto < 2)
			resto = 0;
		else
			resto = 11 - resto;

		digito = digito + resto.ToString();

		return cnpj.EndsWith(digito);
	}

	public string Formatado()
	{
		return $"{Numero.Substring(0, 2)}.{Numero.Substring(2, 3)}.{Numero.Substring(5, 3)}/{Numero.Substring(8, 4)}-{Numero.Substring(12, 2)}";
	}

	public override string ToString()
	{
		return Numero;
	}
}
