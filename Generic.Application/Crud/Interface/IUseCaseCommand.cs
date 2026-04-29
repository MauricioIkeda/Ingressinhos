using Generic.Domain.Entities;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseCommand<in TCommand>
{
    ListMessages Messages { get; }
    bool Execute(TCommand command);
}
