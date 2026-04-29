using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using Ingressinhos.Application.Catalog.Location.UseCases;
using DomainLocation = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseLocationCollection : UseCaseCrudCollection<DomainLocation, LocationDto>, IUseCaseLocationCollection
{
    public UseCaseLocationCollection(IRepositorySession repositorySession, UpdateLocationUseCase update, CreateLocationUseCase include)
        : base(include, update, new UseCaseGetOdata<DomainLocation>(), new UseCaseGet<DomainLocation>(), new UseCaseDelete<DomainLocation>(), repositorySession)
    {
    }
}
