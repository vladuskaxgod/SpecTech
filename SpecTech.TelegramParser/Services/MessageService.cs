using SpecTech.Domain.Entities;
using SpecTech.Domain.Models;
using SpecTech.Domain.Services;
using TL;
using WTelegram;

namespace SpecTech.TelegramParser.Services;

internal class MessageService
{
    private readonly Client _client;
    private readonly TelegramService _telegramService;

    internal MessageService(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<List<TelegramMessageModel>> GetAllMessages(List<long> chatIds)
    {
        var res = new List<TelegramMessageModel>();
        var chats = await _client.Messages_GetAllChats();
        
        foreach (var chatId in chatIds) {
            var messages = await _client.Messages_GetHistory(chats.chats[chatId]);
            if (messages.Messages.Length == 0) continue;
            foreach (var msgBase in messages.Messages)
            {
                var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                if (msgBase is Message msg)
                {
                    Console.WriteLine($"{from}> {msg.message} {msg.media}");
                    res.Add(new TelegramMessageModel()
                    {
                        MessageId = msg.id,
                        UserId = from.ID,
                        ChatId = chatId,
                        SourceMessage = msg.message,
                        Timestamp = msg.date
                    });
                }
                else if (msgBase is TL.MessageService ms)
                    Console.WriteLine($"{from} [{ms.action.GetType().Name[13..]}]");
            }
        }
        return res;
    }

    public async Task UpdateOrInsertTelegramMessage(Message message)
    {
        var user = await _telegramService.GetUserByTelegramIdAsync(message.from_id);
        var chat = await _telegramService.GetChatByTelegramIdAsync(message.peer_id);

        var entity = new TelegramMessageEntity()
        {
            TelegramMessageId = message.id,
            SourceMessage = message.message,
            Timestamp = message.date,
            User = user,
            Chat = chat
        };

        await _telegramService.UpdateOrInsertTelegramMessageAsync(entity);
    }
}