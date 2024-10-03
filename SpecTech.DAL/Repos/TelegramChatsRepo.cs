using Infrastructure.Data;
using SpecTech.Domain.Entities;
using SpecTech.Domain.Interfaces.Repositories;

namespace SpecTech.DAL.Repos;

public class TelegramChatsRepo(ApplicationContext context) : BaseRepo(context), ITelegramChatsRepo
{
    public Task<List<TelegramChatEntity>> GetAllAsync()
    {
        return base.GetAllAsync<TelegramChatEntity>();
    }

    public Task<TelegramChatEntity> CreateAsync(TelegramChatEntity entity)
    {
        return base.CreateAsync(entity);
    }

    public TelegramChatEntity? GetChatByTelegramIdSync(long telegramId)
    {
        return _context.TelegramChats.FirstOrDefault(x => x.TelegramChatId == telegramId);
    }
}