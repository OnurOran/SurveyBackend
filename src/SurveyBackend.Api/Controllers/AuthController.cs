using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Contracts;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Auth.Commands.Admin;
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
    private readonly IValidator<UpdateLocalAdminPasswordCommand> _updatePasswordValidator;

    public AuthController(
        IAppMediator mediator,
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<RefreshTokenRequest> refreshTokenRequestValidator,
        IValidator<UpdateLocalAdminPasswordCommand> updatePasswordValidator)
    {
        _mediator = mediator;
        _loginRequestValidator = loginRequestValidator;
        _refreshTokenRequestValidator = refreshTokenRequestValidator;
        _updatePasswordValidator = updatePasswordValidator;
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

    [AllowAnonymous]
    [HttpPost("admin/password")]
    public async Task<IActionResult> UpdateAdminPassword([FromBody] UpdateAdminPasswordRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Geçersiz istek.");
        }

        var command = new UpdateLocalAdminPasswordCommand(request.NewPassword);
        var validationResult = await _updatePasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddErrorsToModelState(validationResult);
            return ValidationProblem(ModelState);
        }

        await _mediator.SendAsync<UpdateLocalAdminPasswordCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    private void AddErrorsToModelState(FluentValidation.Results.ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
