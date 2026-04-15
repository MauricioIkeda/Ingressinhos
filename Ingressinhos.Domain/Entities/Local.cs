using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Entities;

public class Local : BaseEntity
{
    public string Nome { get; private set; }
    public int CapacidadeTotal { get; private set; }

    public Local(string nome, int capacidadeTotal)
    {
        if (string.IsNullOrEmpty(nome))
        {
            throw new Exception("Deve ser informado o nome do local");
        }

        if (capacidadeTotal <= 0)
        {
            throw new Exception("Deve ser informado uma capacidade valida do local");
        }
        
        Nome = nome;
        CapacidadeTotal = capacidadeTotal;
    }
}