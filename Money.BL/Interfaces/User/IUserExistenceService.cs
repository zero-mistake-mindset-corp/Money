namespace Money.BL.Services.User;

public interface IUserExistenceService
{
    Task EnsureUserDoesNotExist(string email, string username);
    Task EnsureUserDoesNotExist(string usernameOrEmail);
}