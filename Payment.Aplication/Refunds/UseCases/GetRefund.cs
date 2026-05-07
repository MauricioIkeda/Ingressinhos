using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Aplication.Refunds.Utils;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.UseCases;

public class GetRefund : IUseCaseGetRefund
{
    private readonly IRepositorySession _repositorySession;

    public GetRefund(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<RefundDto> Execute(long refundId)
    {
        if (refundId <= 0)
        {
            return OperationResult<RefundDto>.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do reembolso."));
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var refund = repositoryQuery.Return<Refund>(refundId);

        if (refund is null)
        {
            return OperationResult<RefundDto>.NotFound(new MensagemErro("Reembolso", "Reembolso nao encontrado."));
        }

        return OperationResult<RefundDto>.Ok(refund.ToDto());
    }
}
