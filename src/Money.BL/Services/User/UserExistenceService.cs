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

    public async Task EnsureUserDoesNotExist(string username)
    {
        if (await _context.Users.Where(u => u.Username == username).AnyAsync())
        {
            throw new EntityExistsException("User with this username already exists.");
        }
    }
}
