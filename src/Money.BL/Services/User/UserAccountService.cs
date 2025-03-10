using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.User;
using Money.BL.Models.Auth;
using Money.BL.Models.UserAccount;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.User;

public class UserAccountService : IUserAccountService
{
    private readonly AppDbContext _context;
    private readonly IUserExistenceService _userExistenceService;
    private readonly IMapper _mapper;

    public UserAccountService(AppDbContext context, IUserExistenceService userExistenceService, IMapper mapper)
    {
        _context = context;
        _userExistenceService = userExistenceService;
        _mapper = mapper;
    }

    public async Task SignUpAsync(SignUpModel signUpModel)
    {
        await _userExistenceService.EnsureUserDoesNotExist(signUpModel.Username);

        var userEntity = _mapper.Map<UserEntity>(signUpModel);
        userEntity.PasswordHash = InformationHasher.HashText(signUpModel.Password);

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<GetUserProfileModel> GetUserAccountAsync(Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userProfile = _mapper.Map<GetUserProfileModel>(user);

        return userProfile;
    }

    public async Task UpdateAccountAsync(UpdateAccountModel model, Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        await _userExistenceService.EnsureUserDoesNotExist(model.Username);

        _mapper.Map(model, user);
        await _context.SaveChangesAsync();
    }
}
