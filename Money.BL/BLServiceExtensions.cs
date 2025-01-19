using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.BL.Interfaces.Auth;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Interfaces.MoneyAccount;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.TransactionTypes;
using Money.BL.Interfaces.Transfer;
using Money.BL.Interfaces.User;
using Money.BL.Services.Auth;
using Money.BL.Services.Infrastructure;
using Money.BL.Services.MoneyAccount;
using Money.BL.Services.Transactions;
using Money.BL.Services.TransactionTypes;
using Money.BL.Services.Transfer;
using Money.BL.Services.User;

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
        services.AddScoped<IIncomeTransactionService, IncomeTransactionService>();
        services.AddScoped<IExpenseTransactionService, ExpenseTransactionService>();
        services.AddScoped<ITemplateRenderer, TemplateRenderer>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserExistenceService, UserExistenceService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<ITransferService, TransferService>();
        return services;
    }
}
