using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public RefreshTokenRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokens
            .AsTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
