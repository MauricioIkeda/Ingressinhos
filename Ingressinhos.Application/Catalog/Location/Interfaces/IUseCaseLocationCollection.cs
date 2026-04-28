using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Catalog.Location.Dtos;
using DomainLocation = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.Interfaces;

public interface IUseCaseLocationCollection : IUseCaseCrudCollection<DomainLocation, LocationDto>
{
}