using Generic.Domain.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Seat : BaseEntity
{
    public long LocationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public SeatCategory Category { get; private set; }
    public SeatStatus Status { get; private set; }

    protected Seat()
    {
    }

    public Seat(long locationId, string code, SeatCategory category)
    {
        if (locationId <= 0)
        {
            AddError("LocationId", "Deve ser informado o local do assento");
        }
        else
        {
            LocationId = locationId;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            AddError("Code", "Deve ser informado o codigo do assento");
        }
        else
        {
            Code = code.Trim().ToUpperInvariant();
        }

        Category = category;
        Status = SeatStatus.Available;
    }

    public void ChangeCategory(SeatCategory category)
    {
        ClearErrors();
        Category = category;
    }

    public void Reserve()
    {
        ClearErrors();

        if (Status == SeatStatus.Blocked)
        {
            AddError("Status", "Assento bloqueado nao pode ser reservado");
            return;
        }

        if (Status == SeatStatus.Occupied)
        {
            AddError("Status", "Assento ocupado nao pode ser reservado");
            return;
        }

        Status = SeatStatus.Reserved;
    }

    public void Occupy()
    {
        ClearErrors();

        if (Status == SeatStatus.Blocked)
        {
            AddError("Status", "Assento bloqueado nao pode ser ocupado");
            return;
        }

        Status = SeatStatus.Occupied;
    }

    public void Release()
    {
        ClearErrors();

        if (Status == SeatStatus.Blocked)
        {
            AddError("Status", "Assento bloqueado nao pode ser liberado");
            return;
        }

        Status = SeatStatus.Available;
    }

    public void Block()
    {
        ClearErrors();
        Status = SeatStatus.Blocked;
    }

    public void Unblock()
    {
        ClearErrors();
        Status = SeatStatus.Available;
    }
}
