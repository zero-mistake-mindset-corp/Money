namespace Money.Data.Entities;

public class TransferEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public DateTime TrackingDate { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    public Guid? SendingMoneyAccountId { get; set; }
    public virtual MoneyAccountEntity SendingMoneyAccount { get; set; }
    public Guid? ReceivingMoneyAccountId { get; set; }
    public virtual MoneyAccountEntity ReceivingMoneyAccount { get; set; }
}
