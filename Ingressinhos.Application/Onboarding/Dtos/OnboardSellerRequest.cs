namespace Ingressinhos.Application.Onboarding.Dtos;

public sealed record OnboardSellerRequest(
    string Name,
    string Email,
    string Cnpj,
    string TradingName
);
