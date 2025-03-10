namespace Money.BL.Models.Transfer;

public class CreateTransferModel
{
    public string Name { get; set; }
    public string Comment { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public Guid SendingMoneyAccountId { get; set; }
    public Guid ReceivingMoneyAccountId { get; set; }
}
