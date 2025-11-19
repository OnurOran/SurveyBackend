using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshTokenRequestValidator;

    public AuthController(
        IAppMediator mediator,
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<RefreshTokenRequest> refreshTokenRequestValidator)
    {
        _mediator = mediator;
        _loginRequestValidator = loginRequestValidator;
        _refreshTokenRequestValidator = refreshTokenRequestValidator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddErrorsToModelState(validationResult);
            return ValidationProblem(ModelState);
        }

        var command = AuthMapper.ToLoginCommand(request);
        var response = await _mediator.SendAsync<LoginCommand, AuthTokensDto>(command, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _refreshTokenRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddErrorsToModelState(validationResult);
            return ValidationProblem(ModelState);
        }

        var command = AuthMapper.ToRefreshTokenCommand(request);
        var response = await _mediator.SendAsync<RefreshTokenCommand, AuthTokensDto>(command, cancellationToken);
        return Ok(response);
    }

    private void AddErrorsToModelState(FluentValidation.Results.ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
