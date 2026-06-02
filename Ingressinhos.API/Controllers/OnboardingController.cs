using Generic.Domain.Entities;
using Ingressinhos.Application.Onboarding.Dtos;
using Ingressinhos.Application.Onboarding.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/onboarding")]
public sealed class OnboardingController : ControllerBase
{
    private readonly ProfileStatusUseCase _profileStatusUseCase;
    private readonly OnboardClientUseCase _onboardClientUseCase;
    private readonly OnboardSellerUseCase _onboardSellerUseCase;

    public OnboardingController(
        ProfileStatusUseCase profileStatusUseCase,
        OnboardClientUseCase onboardClientUseCase,
        OnboardSellerUseCase onboardSellerUseCase)
    {
        _profileStatusUseCase = profileStatusUseCase;
        _onboardClientUseCase = onboardClientUseCase;
        _onboardSellerUseCase = onboardSellerUseCase;
    }

    [HttpGet("profile-status")]
    public IActionResult ProfileStatus()
    {
        return Handle(_profileStatusUseCase.Execute());
    }

    [HttpPost("client")]
    public IActionResult Client(OnboardClientRequest request)
    {
        return Handle(_onboardClientUseCase.Execute(request));
    }

    [HttpPost("seller")]
    public IActionResult Seller(OnboardSellerRequest request)
    {
        return Handle(_onboardSellerUseCase.Execute(request));
    }

    private IActionResult Handle(OperationResult result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }

    private IActionResult Handle<T>(OperationResult<T> result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
