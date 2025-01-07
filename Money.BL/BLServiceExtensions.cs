using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.BL.Interfaces;
using Money.BL.Interfaces.Auth;
using Money.BL.Services;
using Money.BL.Services.Auth;

namespace Money.BL;

public static class BLServiceExtensions
{
    public static IServiceCollection ConfigureBLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMoneyAccountService, MoneyAccountService>();
        services.AddScoped<IIncomeTypeService, IncomeTypeService>();
        return services;
    }
}
