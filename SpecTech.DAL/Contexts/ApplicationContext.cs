using Microsoft.EntityFrameworkCore;
using SpecTech.Domain.Entities;

namespace Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext() : base(GetOptions())
    {
    }
    
    private static DbContextOptions GetOptions()
    {
        return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), "Server=(localdb)\\MSSQLLocalDB;Database=parser;Trusted_Connection=True;TrustServerCertificate=True;").Options;
    }
    
    public DbSet<TelegramMessageEntity> TelegramMessages { get; set; }
    public DbSet<TelegramUserEntity> TelegramUsers { get; set; }
    public DbSet<TelegramChatEntity> TelegramChats { get; set; }
}