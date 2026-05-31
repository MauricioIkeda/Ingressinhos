namespace Ingressinhos.Application.Onboarding.Dtos;

public sealed record ProfileStatusDto(
    bool HasClientProfile,
    bool HasSellerProfile,
    long? ClientId,
    long? SellerId
);
