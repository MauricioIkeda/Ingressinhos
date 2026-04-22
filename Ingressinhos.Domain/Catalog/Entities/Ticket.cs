using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Ticket : BaseEntity
{
    public long EventId { get; private set; }
    public long SellerId { get; private set; }
    public string Name { get; private set; }
    public Price BasePrice { get; private set; }
    public Price? PremiumPrice { get; private set; }
    public Price? VIPPrice { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }
    public DateTime SalesStartsAt { get; private set; }
    public DateTime SalesEndsAt { get; private set; }
    public CatalogTicketStatus Status { get; private set; }

    protected Ticket()
    {
        
    }
    
    public Ticket(long eventId, string name, decimal basePrice, decimal? premiumPrice, decimal? vipPrice, int totalQuantity, DateTime salesStartsAt, DateTime salesEndsAt)
    {
        if (eventId <= 0)
        {
            throw new Exception("Deve ser informado o evento do ingresso");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do ingresso");
        }

        if (totalQuantity <= 0)
        {
            throw new Exception("A quantidade total do ingresso deve ser maior que zero");
        }

        if (salesEndsAt <= salesStartsAt)
        {
            throw new Exception("O fim das vendas deve ser posterior ao inicio");
        }

        EventId = eventId;
        Name = name.Trim();
        BasePrice = new Price(basePrice);
        PremiumPrice = premiumPrice.HasValue ? new Price(premiumPrice.Value) : null;
        VIPPrice = vipPrice.HasValue ? new Price(vipPrice.Value) : null;
        TotalQuantity = totalQuantity;
        AvailableQuantity = totalQuantity;
        SalesStartsAt = salesStartsAt;
        SalesEndsAt = salesEndsAt;
        Status = CatalogTicketStatus.Active;
    }

    public void ChangePrices(decimal? newBasePrice, decimal? newPremiumPrice,  decimal? newVIPPrice)
    {
        BasePrice = newBasePrice.HasValue ? new Price(newBasePrice.Value) : BasePrice;
        PremiumPrice = newPremiumPrice.HasValue ? new Price(newPremiumPrice.Value) : PremiumPrice;
        VIPPrice = newVIPPrice.HasValue ? new Price(newVIPPrice.Value) : VIPPrice;
    }

    public void Reserve(int quantity, DateTime referenceDate)
    {
        EnsureTicketCanSell(referenceDate);

        if (quantity <= 0)
        {
            throw new Exception("A quantidade deve ser maior que zero");
        }

        if (quantity > AvailableQuantity)
        {
            throw new Exception("Nao ha ingressos suficientes disponiveis");
        }

        AvailableQuantity -= quantity;

        if (AvailableQuantity == 0)
        {
            Status = CatalogTicketStatus.SoldOut;
        }
    }

    public void RestoreQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception("A quantidade deve ser maior que zero");
        }

        if (AvailableQuantity + quantity > TotalQuantity)
        {
            throw new Exception("A quantidade restaurada excede o total de ingressos");
        }

        AvailableQuantity += quantity;

        if (Status == CatalogTicketStatus.SoldOut && AvailableQuantity > 0)
        {
            Status = CatalogTicketStatus.Active;
        }
    }

    public void Disable()
    {
        Status = CatalogTicketStatus.Inactive;
    }

    public void Enable()
    {
        Status = AvailableQuantity > 0 ? CatalogTicketStatus.Active : CatalogTicketStatus.SoldOut;
    }

    public void AddCapacity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception("A quantidade deve ser maior que zero");
        }

        TotalQuantity += quantity;
        AvailableQuantity += quantity;

        if (Status == CatalogTicketStatus.SoldOut)
        {
            Status = CatalogTicketStatus.Active;
        }
    }

    private void EnsureTicketCanSell(DateTime referenceDate)
    {
        if (Status == CatalogTicketStatus.Inactive)
        {
            throw new Exception("O ingresso esta inativo");
        }

        if (Status == CatalogTicketStatus.SoldOut)
        {
            throw new Exception("O ingresso esta esgotado");
        }

        if (referenceDate < SalesStartsAt || referenceDate > SalesEndsAt)
        {
            throw new Exception("O ingresso esta fora da janela de venda");
        }
    }
}