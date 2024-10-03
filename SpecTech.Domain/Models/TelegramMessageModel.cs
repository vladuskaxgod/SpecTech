namespace SpecTech.Domain.Models;

public class TelegramMessageModel
{
    public long MessageId { get; set; }
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string SourceMessage { get; set; }
    public DateTime Timestamp { get; set; }
    
    // TODO: attachments, stickers, etc
}