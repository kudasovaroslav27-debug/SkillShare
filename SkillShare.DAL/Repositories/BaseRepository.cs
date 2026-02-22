using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.DAL.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _DbContex;

    public BaseRepository(ApplicationDbContext dbContex)
    {
        _DbContex = dbContex;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _DbContex.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("Entity is null");

          await _DbContex.AddAsync(entity);

        return entity;
    }

    public void Remove(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("Entity is null");

        _DbContex.Remove(entity);
    }

    public  TEntity Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("Entity is null");

        _DbContex.Update(entity);

        return entity;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _DbContex.SaveChangesAsync();
    }
}
