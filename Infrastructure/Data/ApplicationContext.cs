using BLL.Entities;
using Microsoft.EntityFrameworkCore;


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
    
    public DbSet<MessageEntity> Messages { get; set; }
}