using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.Transaction;

public class UpdateIncomeTransactionModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name max length is 100 symbols")]
    public string Name { get; set; }

    [MaxLength(100, ErrorMessage = "Comment max length is 100 symbols")]
    public string Comment { get; set; }

    [Required(ErrorMessage = "Transaction date is required")]
    public DateTime? TransactionDate { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be > 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Money account id is required")]
    public Guid? MoneyAccountId { get; set; }

    [Required(ErrorMessage = "Expense type id is required")]
    public Guid? ExpenseTypeId { get; set; }
}
