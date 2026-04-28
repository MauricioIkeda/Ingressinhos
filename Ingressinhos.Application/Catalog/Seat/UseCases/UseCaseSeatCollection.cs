using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseSeatCollection : UseCaseCrudCollection<Seat, SeatDto>, IUseCaseSeatCollection
{
    public UseCaseSeatCollection(IRepositorySession repositorySession, SeatUpdate update, SeatInclude include)
        : base(include.Execute, update.Execute, new UseCaseGetOdata<Seat>(), new UseCaseGet<Seat>(), new UseCaseDelete<Seat>(), repositorySession)
    {
    }
}