using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private DbSet<TEntity> _entity;

        public Repository(DbContext context)
        {
            this.Context = context;
            _entity = this.Context.Set<TEntity>();
        }

        public Task<TEntity> GetAsync(int id)
        {
            return _entity.FindAsync(id);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _entity.ToListAsync();
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entity.Where(predicate).ToListAsync();
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entity.SingleOrDefaultAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            _entity.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entity.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            _entity.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            _entity.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entity.RemoveRange(entities);
        }
    }
}
