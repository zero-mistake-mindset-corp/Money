using Microsoft.EntityFrameworkCore;
using Money.Common.Exceptions;
using Money.Data;

namespace Money.BL.Services.User;

public class UserExistenceService : IUserExistenceService
{
    private readonly AppDbContext _context;

    public UserExistenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task EnsureUserDoesNotExist(string email, string username)
    {
        if (await _context.Users.Where(u => u.Email == email || u.Username == username || u.Email == username || u.Username == email).AnyAsync())
        {
            throw new EntityExistsException("User with this username or email already exists.");
        }
    }

    public async Task EnsureUserDoesNotExist(string usernameOrEmail)
    {
        if (await _context.Users.Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail).AnyAsync())
        {
            throw new EntityExistsException("User with this username or email already exists.");
        }
    }
}
