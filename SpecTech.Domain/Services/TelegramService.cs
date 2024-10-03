using SpecTech.Domain.Entities;
using SpecTech.Domain.Interfaces.Repositories;

namespace SpecTech.Domain.Services;

public class TelegramService
{
    private readonly ITelegramMessagesRepo _telegramMessagesRepo;
    private readonly ITelegramUsersRepo _telegramUsersRepo;
    private readonly ITelegramChatsRepo _telegramChatsRepo;

    public TelegramService(ITelegramMessagesRepo telegramMessagesRepo, ITelegramUsersRepo telegramUsersRepo, ITelegramChatsRepo telegramChatsRepo)
    {
        _telegramMessagesRepo = telegramMessagesRepo;
        _telegramUsersRepo = telegramUsersRepo;
        _telegramChatsRepo = telegramChatsRepo;
    }

    public async Task<List<TelegramMessageEntity>> GetAllTelegramMessagesAsync()
    {
        return await _telegramMessagesRepo.GetAllAsync();
    }

    public async Task<TelegramMessageEntity> UpdateOrInsertTelegramMessageAsync(TelegramMessageEntity entity)
    {
        return await _telegramMessagesRepo.UpdateOrInsertAsync(entity);
    }

    public async Task<TelegramUserEntity> GetUserByTelegramIdAsync(long telegramId)
    {
        var user = _telegramUsersRepo.GetUserByTelegramIdSync(telegramId);

        if (user == null)
        {
            user = await _telegramUsersRepo.CreateAsync(new TelegramUserEntity()
            {
                TelegramUserId = telegramId,
            });
        }

        return user;
    }

    public async Task<TelegramChatEntity> GetChatByTelegramIdAsync(long telegramId)
    {
        var chat = _telegramChatsRepo.GetChatByTelegramIdSync(telegramId);
        
        if (chat == null)
        {
            chat = await _telegramChatsRepo.CreateAsync(new TelegramChatEntity()
            {
                TelegramChatId = telegramId,
            });
        }

        return chat;
    }
}