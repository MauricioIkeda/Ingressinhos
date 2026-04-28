using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.Interfaces;

public interface IUseCaseEventCollection : IUseCaseCrudCollection<Event, EventDto>
{
}