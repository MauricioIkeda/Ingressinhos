using Ingressinhos.Application.Sales.TicketReadModel.Dtos;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IClientTicketReadModelQuery // Query do Mongo DB
{
    //Agora coloquei get genérico para ser reaproveitado com getodata, não especifico para me, agora só especificar no usecase
    IReadOnlyCollection<TOutput> Get<TOutput>(Func<IQueryable<ClientTicketViewDto>, IQueryable<TOutput>> query);
}
