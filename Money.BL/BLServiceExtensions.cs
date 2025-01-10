using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.BL.Interfaces;
using Money.BL.Interfaces.Auth;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Services;
using Money.BL.Services.Auth;
using Money.BL.Services.Infrastructure;

namespace Money.BL;

public static class BLServiceExtensions
{
    public static IServiceCollection ConfigureBLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMoneyAccountService, MoneyAccountService>();
        services.AddScoped<IIncomeTypeService, IncomeTypeService>();
        services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITemplateRenderer, TemplateRenderer>();
        return services;
    }
}
