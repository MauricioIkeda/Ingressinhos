using Generic.Application.Interface;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.Interfaces;

public interface IUseCaseSeatCollection : IUseCaseCrudCollection<Seat, SeatDto>
{
}