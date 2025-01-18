using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Money.BL.Interfaces.Auth;
using Money.BL.Models.Auth;
using Money.Common.Options;
using Money.Data;
using Money.Data.Entities;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Money.BL.Services.Auth;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleOptions _options;
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public GoogleAuthService(AppDbContext context, IAuthService authService, IOptions<GoogleOptions> googleOptions)
    {
        _context = context;
        _authService = authService;
        _options = googleOptions.Value;
    }

    public async Task<TokensInfo> GoogleSignIn(GoogleSignInModel signInModel)
    {
        var payload = await ValidateAsync(signInModel.Value, new ValidationSettings
        {
            Audience = new[] { _options.ClientId }
        });

        var user = await _context.Users.Where(u => u.Email == payload.Email).FirstOrDefaultAsync();
        if (user == null)
        {
            var fullName = string.Join(" ", payload.GivenName, payload.FamilyName);
            user = await CreateUserAsync(payload.Email, fullName);
        }

        var tokensInfo = await _authService.GenerateAndSaveTokensAsync(user);
        return tokensInfo;
    }

    private async Task<UserEntity> CreateUserAsync(string email, string fullname)
    {
        var user = new UserEntity
        {
            Username = fullname,
            Email = email,
            IsEmailConfirmed = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
