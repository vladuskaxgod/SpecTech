using Infrastructure.Data;
using SpecTech.Domain.Entities;
using SpecTech.Domain.Interfaces.Repositories;

namespace SpecTech.DAL.Repos;

public class TelegramUsersRepo(ApplicationContext context) : BaseRepo(context), ITelegramUsersRepo
{
    public Task<List<TelegramUserEntity>> GetAllAsync()
    {
        return base.GetAllAsync<TelegramUserEntity>();
    }

    public Task<TelegramUserEntity> CreateAsync(TelegramUserEntity entity)
    {
        return base.CreateAsync(entity);
    }

    public TelegramUserEntity? GetUserByTelegramIdSync(long telegramId)
    {
        return _context.TelegramUsers.FirstOrDefault(x => x.TelegramUserId == telegramId);
    }
}