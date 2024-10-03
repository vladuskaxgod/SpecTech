using SpecTech.Domain.Entities;

namespace SpecTech.Domain.Interfaces.Repositories;

public interface ITelegramUsersRepo 
{
    public Task<List<TelegramUserEntity>> GetAllAsync();
    public Task<TelegramUserEntity> CreateAsync(TelegramUserEntity entity);
    public TelegramUserEntity? GetUserByTelegramIdSync(long telegramId);
}