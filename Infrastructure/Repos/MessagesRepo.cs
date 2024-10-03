using BLL.Interfaces;
using BLL.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Repos;

public class MessagesRepo : IMessagesRepo
{
    private readonly ApplicationContext _context;

    public MessagesRepo(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task<List<MessageEntity>> GetAllRecordsAsync()
    {
        return await _context.Messages.ToListAsync();
    }
}