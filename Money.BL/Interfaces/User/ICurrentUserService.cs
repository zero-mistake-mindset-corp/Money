namespace Money.BL.Interfaces.User;

public interface ICurrentUserService
{
    Guid GetUserId();
}