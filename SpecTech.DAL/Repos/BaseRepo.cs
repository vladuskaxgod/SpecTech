using Infrastructure.Data;
using SpecTech.Domain.Entities;

namespace SpecTech.DAL.Repos;

public class BaseRepo
{
    internal ApplicationContext _context;

    internal BaseRepo(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        return _context.Set<T>().ToList();
    }

    public async Task<T> CreateAsync<T>(T entity) where T : class
    {
        // TODO: check if Id is tracked and EF is returning value auto-incremented
        
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public async Task<T> UpdateOrInsert<T>(T entity) where T : class
    {
        // TODO: check if Id is tracked and EF is returning value auto-incremented
        
        var dbSet = _context.Set<T>();

        if (dbSet.Any(x => x.Equals(entity)))
        {
            dbSet.Update(entity);
        }
        else
        {
            dbSet.Add(entity);
        }
        
        await _context.SaveChangesAsync();

        return entity;
    }
}