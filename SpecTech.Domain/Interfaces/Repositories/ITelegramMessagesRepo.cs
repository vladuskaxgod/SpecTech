using SpecTech.Domain.Entities;

namespace SpecTech.Domain.Interfaces.Repositories;

public interface ITelegramMessagesRepo 
{
    public Task<List<TelegramMessageEntity>> GetAllAsync();
    public Task<TelegramMessageEntity> UpdateOrInsertAsync(TelegramMessageEntity entity);
}