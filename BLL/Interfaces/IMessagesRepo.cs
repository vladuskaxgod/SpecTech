using BLL.Entities;

namespace BLL.Interfaces;

public interface IMessagesRepo
{
    Task<List<MessageEntity>> GetAllRecordsAsync();
}