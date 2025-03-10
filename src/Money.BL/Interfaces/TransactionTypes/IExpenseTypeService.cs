using Money.BL.Models.TransactionTypes;
using Money.BL.Models.Type;

namespace Money.BL.Interfaces.TransactionTypes;

public interface IExpenseTypeService
{
    Task CreateExpenseTypeAsync(CreateExpenseTypeModel model, Guid userId);

    Task<List<ExpenseTypeModel>> GetAllExpenseTypesAsync(Guid userId);

    Task UpdateExpenseTypeAsync(Guid userId, UpdateExpenseTypeModel model);

    Task DeleteExpenseTypeAsync(Guid userId, Guid expenseTypeId);
}
