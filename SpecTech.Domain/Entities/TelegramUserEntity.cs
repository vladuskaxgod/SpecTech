namespace SpecTech.Domain.Entities;

public class TelegramUserEntity : BaseEntity
{
    public long TelegramUserId { get; set; }

    public virtual List<TelegramMessageEntity> Messages { get; set; } = new();
    public virtual List<TelegramChatEntity> Chats { get; set; } = new();
}