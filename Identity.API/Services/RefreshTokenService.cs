using Identity.API.Data;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services;

public class RefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async void SaveRefreshTokenAsync(RefreshToken? refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);
    }

    public async void RemoveRefreshToken(string token)
    {
        var refreshToken = await GetRefreshTokenAsync(token);
        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }
    }
}