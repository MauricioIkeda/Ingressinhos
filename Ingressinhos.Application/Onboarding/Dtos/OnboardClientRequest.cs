namespace Ingressinhos.Application.Onboarding.Dtos;

public sealed record OnboardClientRequest(
    string Name,
    string Email,
    string Cpf
);
