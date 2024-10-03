using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SpecTech.Domain.Entities;
using SpecTech.Domain.Interfaces.Repositories;

namespace SpecTech.DAL.Repos;

public class TelegramMessagesRepo(ApplicationContext context) : BaseRepo(context), ITelegramMessagesRepo
{
    public Task<List<TelegramMessageEntity>> GetAllAsync()
    {
        return base.GetAllAsync<TelegramMessageEntity>();
    }

    public Task<TelegramMessageEntity> UpdateOrInsertAsync(TelegramMessageEntity entity)
    {
        return base.UpdateOrInsert(entity);
    }
    
}