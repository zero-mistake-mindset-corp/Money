namespace Money.Data.Entities;
public class TransferEntity
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public DateTime TrackingDate { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }
    public Guid SendingMoneyAccountId { get; set; }
    public virtual MoneyAccountEntity SendingMoneyAccount { get; set; }
    public Guid RecievingMoneyAccountId { get; set; }
    public virtual MoneyAccountEntity RecievingMoneyAccount { get; set; }
}
