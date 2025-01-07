namespace Money.Data.Entities;

public class IncomeCategoryEntity
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
}
