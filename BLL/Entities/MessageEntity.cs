namespace BLL.Entities;

public class MessageEntity
{
    public long Id { get; set; }
    public string SourceMessage { get; set; }
    public string TechnicType { get; set; }
    public string Brand  { get; set; }
    public string Model { get; set; }
    public string Engine { get; set; }
    public string Kovsh { get; set; }
    public string CanPull { get; set; }
    public string AdditionalInfo { get; set; }
    public string Contact { get; set; }
    public string Available { get; set; }
}