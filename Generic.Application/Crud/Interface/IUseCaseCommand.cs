using Generic.Domain.Entities;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseCommand<in TCommand>
{
    OperationResult Execute(TCommand command);
}

public interface IUseCaseCommand<in TCommand, TResult>
{
    OperationResult<TResult> Execute(TCommand command);
}
