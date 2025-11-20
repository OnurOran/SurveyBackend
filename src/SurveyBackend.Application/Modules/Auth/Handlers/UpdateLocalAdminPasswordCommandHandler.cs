using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Admin;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class UpdateLocalAdminPasswordCommandHandler : ICommandHandler<UpdateLocalAdminPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateLocalAdminPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> HandleAsync(UpdateLocalAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        var adminUser = await _userRepository.GetLocalByUsernameAsync("admin", cancellationToken);
        if (adminUser is null)
        {
            throw new InvalidOperationException("Yerel admin kullanıcısı bulunamadı.");
        }

        var hash = _passwordHasher.Hash(request.NewPassword);
        adminUser.UpdateLocalPassword(hash, DateTimeOffset.UtcNow);
        await _userRepository.UpdateAsync(adminUser, cancellationToken);
        return true;
    }
}
