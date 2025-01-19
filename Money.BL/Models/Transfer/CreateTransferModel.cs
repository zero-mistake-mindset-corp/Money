namespace Money.BL.Models.Transfer;

public class CreateTransferModel
{
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public string? Comment { get; set; }
    public Guid SendingMoneyAccountId { get; set; }
    public Guid RecievingMoneyAccountId { get; set; }
}
