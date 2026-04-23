using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.Dtos;

public class SeatDto
{
    public long SeatId { get; set; }
    public long LocationId { get; set; }  //Apenas no inclui, nunca permitir editar
    public string Code { get; set; }  // Apenas no inclui, nunca permitir editar
    public SeatCategory Category { get; set; }
    public SeatStatus Status { get; set; }
}