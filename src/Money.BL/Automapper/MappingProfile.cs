using AutoMapper;
using Money.BL.Models.Auth;
using Money.BL.Models.MoneyAccount;
using Money.BL.Models.Transaction;
using Money.BL.Models.TransactionTypes;
using Money.BL.Models.Type;
using Money.BL.Models.UserAccount;
using Money.Common.Helpers;
using Money.Data.Entities;

namespace Money.BL.Automapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SignUpModel, UserEntity>()
            .ForMember(u => u.PasswordHash, opt => opt.Ignore());

        CreateMap<UserEntity, GetUserProfileModel>();
        CreateMap<UpdateAccountModel, UserEntity>();
        CreateMap<CreateMoneyAccountModel, MoneyAccountEntity>();
        CreateMap<MoneyAccountEntity, MoneyAccountModel>();
        CreateMap<UpdateMoneyAccountModel, MoneyAccountEntity>();
        CreateMap<CreateExpenseTypeModel, ExpenseTypeEntity>();
        CreateMap<ExpenseTypeEntity, ExpenseTypeModel>();
        CreateMap<UpdateExpenseTypeModel, ExpenseTypeEntity>();
        CreateMap<CreateIncomeTypeModel, IncomeTypeEntity>();
        CreateMap<IncomeTypeEntity, IncomeTypeModel>();
        CreateMap<UpdateIncomeTypeModel, IncomeTypeEntity>();

        CreateMap<CreateExpenseTransactionModel, ExpenseTransactionEntity>();
        CreateMap<ExpenseTransactionEntity, ExpenseTransactionModel>();
        CreateMap<UpdateExpenseTransactionModel, ExpenseTransactionEntity>();

        CreateMap<CreateIncomeTransactionModel, IncomeTransactionEntity>();
        CreateMap<IncomeTransactionEntity, IncomeTransactionModel>();
        CreateMap<UpdateIncomeTransactionModel, ExpenseTransactionEntity>();
    }
}
