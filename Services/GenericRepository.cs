using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Panis.Services
{
    public class GenericRepository<TEntity>where TEntity:class
    {
        internal DbContext _db;
        internal DbSet<TEntity> _dbSet;
        public GenericRepository(DbContext db)
        {
            _db = db;
            _dbSet = db.Set<TEntity>();
        }

        public IEnumerable<TEntity> All()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }
        public TEntity GetByID(int id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.Property(item, typeof(TEntity).Name + "ID");
            var value = Expression.Constant(id);
            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return _dbSet.AsNoTracking().SingleOrDefault(lambda);

        }
        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> results = _dbSet.AsNoTracking()
              .Where(predicate).ToList();
            return results;
        }
        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var entity = GetByID(id);
            _dbSet.Remove(entity);
        }
    }
}