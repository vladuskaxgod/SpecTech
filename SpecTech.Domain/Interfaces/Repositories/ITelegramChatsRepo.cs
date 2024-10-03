using SpecTech.Domain.Entities;

namespace SpecTech.Domain.Interfaces.Repositories;

public interface ITelegramChatsRepo
{
    public Task<List<TelegramChatEntity>> GetAllAsync();
    public Task<TelegramChatEntity> CreateAsync(TelegramChatEntity entity);
    public TelegramChatEntity? GetChatByTelegramIdSync(long telegramId);
}