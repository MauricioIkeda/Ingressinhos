using Ingressinhos.Domain.Catalog.Enums;
using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Seat : BaseEntity
{
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; }
    public string Code { get; private set; }
    public SeatCategory Category { get; private set; }
    public SeatStatus Status { get; private set; }

    protected Seat()
    {
        
    }

    public Seat(Guid locationId, string code, SeatCategory category)
    {
        if (locationId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o local do assento");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new Exception("Deve ser informado o codigo do assento");
        }

        Id = Guid.NewGuid();
        LocationId = locationId;
        Code = code.Trim().ToUpperInvariant();
        Category = category;
        Status = SeatStatus.Available;
    }

    public void ChangeCategory(SeatCategory category)
    {
        Category = category;
    }

    public void Reserve()
    {
        if (Status == SeatStatus.Blocked)
        {
            throw new Exception("Assento bloqueado nao pode ser reservado");
        }

        if (Status == SeatStatus.Occupied)
        {
            throw new Exception("Assento ocupado nao pode ser reservado");
        }

        Status = SeatStatus.Reserved;
    }

    public void Occupy()
    {
        if (Status == SeatStatus.Blocked)
        {
            throw new Exception("Assento bloqueado nao pode ser ocupado");
        }

        Status = SeatStatus.Occupied;
    }

    public void Release()
    {
        if (Status == SeatStatus.Blocked)
        {
            throw new Exception("Assento bloqueado nao pode ser liberado");
        }

        Status = SeatStatus.Available;
    }

    public void Block()
    {
        Status = SeatStatus.Blocked;
    }

    public void Unblock()
    {
        Status = SeatStatus.Available;
    }
}