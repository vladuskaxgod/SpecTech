namespace SpecTech.Domain.Entities;

public class TelegramChatEntity : BaseEntity
{
    public string Name { get; set; }
    public long TelegramChatId { get; set; }

    public virtual List<TelegramMessageEntity> Messages { get; set; } = new();
    public virtual List<TelegramUserEntity> Users { get; set; } = new();
}