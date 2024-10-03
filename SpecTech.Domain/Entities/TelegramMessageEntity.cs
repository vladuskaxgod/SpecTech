namespace SpecTech.Domain.Entities;

public class TelegramMessageEntity : BaseEntity
{
    public long TelegramMessageId { get; set; }
    public string SourceMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime? EditTimestamp { get; set; }
    
    // TODO: attachments, stickers, etc

    public virtual TelegramUserEntity User { get; set; }
    public virtual TelegramChatEntity Chat { get; set; }
}