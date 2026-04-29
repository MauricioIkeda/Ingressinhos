using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Ticket : BaseEntity
{
    public long EventId { get; private set; }
    public long SellerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Price BasePrice { get; private set; } = new(0);
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
    
    public Ticket(long eventId, long sellerId, string name, decimal basePrice, decimal? premiumPrice, decimal? vipPrice, int totalQuantity, DateTime salesStartsAt, DateTime salesEndsAt)
    {
        if (eventId <= 0)
        {
            AddError("EventId", "Deve ser informado o evento do ingresso");
        }
        else
        {
            EventId = eventId;
        }

        SellerId = sellerId;

        if (string.IsNullOrWhiteSpace(name))
        {
            AddError("Name", "Deve ser informado o nome do ingresso");
        }
        else
        {
            Name = name.Trim();
        }

        if (totalQuantity <= 0)
        {
            AddError("TotalQuantity", "A quantidade total do ingresso deve ser maior que zero");
        }
        else
        {
            TotalQuantity = totalQuantity;
            AvailableQuantity = totalQuantity;
        }

        if (salesEndsAt <= salesStartsAt)
        {
            AddError("SalesEndsAt", "O fim das vendas deve ser posterior ao inicio");
        }
        else
        {
            SalesStartsAt = salesStartsAt;
            SalesEndsAt = salesEndsAt;
        }

        var basePriceValue = new Price(basePrice);
        CopyErrorsFrom(basePriceValue);
        if (basePriceValue.IsValid)
        {
            BasePrice = basePriceValue;
        }

        if (premiumPrice.HasValue)
        {
            var premium = new Price(premiumPrice.Value);
            CopyErrorsFrom(premium);
            if (premium.IsValid)
            {
                PremiumPrice = premium;
            }
        }

        if (vipPrice.HasValue)
        {
            var vip = new Price(vipPrice.Value);
            CopyErrorsFrom(vip);
            if (vip.IsValid)
            {
                VIPPrice = vip;
            }
        }

        Status = CatalogTicketStatus.Active;
    }

    public void ChangePrices(decimal? newBasePrice, decimal? newPremiumPrice, decimal? newVIPPrice)
    {
        ClearErrors();

        if (newBasePrice.HasValue)
        {
            var basePrice = new Price(newBasePrice.Value);
            CopyErrorsFrom(basePrice);
            if (basePrice.IsValid)
            {
                BasePrice = basePrice;
            }
        }

        if (newPremiumPrice.HasValue)
        {
            var premiumPrice = new Price(newPremiumPrice.Value);
            CopyErrorsFrom(premiumPrice);
            if (premiumPrice.IsValid)
            {
                PremiumPrice = premiumPrice;
            }
        }

        if (newVIPPrice.HasValue)
        {
            var vipPrice = new Price(newVIPPrice.Value);
            CopyErrorsFrom(vipPrice);
            if (vipPrice.IsValid)
            {
                VIPPrice = vipPrice;
            }
        }
    }

    public void Reserve(int quantity, DateTime referenceDate)
    {
        ClearErrors();
        EnsureTicketCanSell(referenceDate);
        if (!IsValid)
        {
            return;
        }

        if (quantity <= 0)
        {
            AddError("Quantity", "A quantidade deve ser maior que zero");
            return;
        }

        if (quantity > AvailableQuantity)
        {
            AddError("Quantity", "Nao ha ingressos suficientes disponiveis");
            return;
        }

        AvailableQuantity -= quantity;

        if (AvailableQuantity == 0)
        {
            Status = CatalogTicketStatus.SoldOut;
        }
    }

    public void RestoreQuantity(int quantity)
    {
        ClearErrors();

        if (quantity <= 0)
        {
            AddError("Quantity", "A quantidade deve ser maior que zero");
            return;
        }

        if (AvailableQuantity + quantity > TotalQuantity)
        {
            AddError("Quantity", "A quantidade restaurada excede o total de ingressos");
            return;
        }

        AvailableQuantity += quantity;

        if (Status == CatalogTicketStatus.SoldOut && AvailableQuantity > 0)
        {
            Status = CatalogTicketStatus.Active;
        }
    }

    public void Disable()
    {
        ClearErrors();
        Status = CatalogTicketStatus.Inactive;
    }

    public void Enable()
    {
        ClearErrors();
        Status = AvailableQuantity > 0 ? CatalogTicketStatus.Active : CatalogTicketStatus.SoldOut;
    }

    public void AddCapacity(int quantity)
    {
        ClearErrors();

        if (quantity <= 0)
        {
            AddError("Quantity", "A quantidade deve ser maior que zero");
            return;
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
            AddError("Status", "O ingresso esta inativo");
            return;
        }

        if (Status == CatalogTicketStatus.SoldOut)
        {
            AddError("Status", "O ingresso esta esgotado");
            return;
        }

        if (referenceDate < SalesStartsAt || referenceDate > SalesEndsAt)
        {
            AddError("SalesWindow", "O ingresso esta fora da janela de venda");
        }
    }
}
