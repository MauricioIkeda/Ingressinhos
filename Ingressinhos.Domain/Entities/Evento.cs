using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Entities;

public class Evento : BaseEntity
{
    public string Nome { get; private set; }
    public DateTime DataHora { get; private set; }
    public Guid LocalId { get; private set; }

    public Evento(string nome, DateTime dataHora, Guid localId)
    {
        if (string.IsNullOrEmpty(nome))
        {
            throw new Exception("Deve ser informado o nome do evento");
        }

        if (dataHora <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida para o evento");
        }

        if (localId == Guid.Empty)
        {
            throw  new Exception("Deve ser informado uma localidade");
        }
        
        Nome = nome;
        DataHora = dataHora;
        LocalId = localId;
    }

    public void RemarcarEvento(DateTime novaDataHora)
    {
        if (novaDataHora <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida pro futuro");
        }
        
        DataHora = novaDataHora;
    }
}