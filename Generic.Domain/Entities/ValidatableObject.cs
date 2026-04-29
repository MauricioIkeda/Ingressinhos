using System.ComponentModel.DataAnnotations.Schema;

namespace Generic.Domain.Entities;

public abstract class ValidatableObject
{
    [NotMapped]
    public List<MensagemErro> Errors { get; } = [];

    [NotMapped]
    public bool IsValid => Errors.Count == 0;

    protected void ClearErrors()
    {
        Errors.Clear();
    }

    protected void AddError(string propriedade, string mensagem)
    {
        Errors.Add(new MensagemErro(propriedade, mensagem));
    }

    protected void AddError(string mensagem)
    {
        Errors.Add(MensagemErro.Geral(mensagem));
    }

    protected void AddErrors(IEnumerable<MensagemErro> errors)
    {
        Errors.AddRange(errors.Where(e => e is not null));
    }

    protected void CopyErrorsFrom(ValidatableObject value)
    {
        if (value == null || value.IsValid)
        {
            return;
        }

        AddErrors(value.Errors);
    }

    public OperationResult ToUnprocessableEntityResult()
    {
        return OperationResult.UnprocessableEntity(Errors.Any() ? Errors : [MensagemErro.Geral("Objeto de dominio invalido.")]);
    }
}
