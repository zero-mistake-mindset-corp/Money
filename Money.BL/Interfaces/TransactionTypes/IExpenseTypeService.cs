using Money.BL.Models.Type;

namespace Money.BL.Interfaces.TransactionTypes;

public interface IExpenseTypeService
{
    Task CreateExpenseTypeAsync(CreateExpenseTypeModel model, Guid userId);

    Task<List<ExpenseTypeModel>> GetAllExpenseTypesAsync(Guid userId);

    Task UpdateExpenseTypeAsync(Guid userId, Guid expenseTypeId, string newExpenseTypeName);

    Task DeleteExpenseTypeAsync(Guid userId, Guid expenseTypeId);
}
