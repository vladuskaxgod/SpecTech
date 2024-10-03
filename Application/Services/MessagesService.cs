using AutoMapper;
using BLL.Interfaces;
using BLL.Entities;

namespace Application.Services;

public class MessagesService
{
    private readonly IMessagesRepo _messagesRepo;
    private readonly IMapper _mapper;

    public MessagesService(IMessagesRepo messagesRepo, IMapper mapper)
    {
        _messagesRepo = messagesRepo;
        _mapper = mapper;
    }

    public async Task<List<MessageEntity>> GetAllRecordsAsync()
    {
        var messages = await _messagesRepo.GetAllRecordsAsync();
        return _mapper.Map<List<MessageEntity>>(messages);
    }
}